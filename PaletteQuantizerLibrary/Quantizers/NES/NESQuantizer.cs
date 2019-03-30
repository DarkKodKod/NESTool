#define UseDictionary

using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using SimplePaletteQuantizer.ColorCaches;
using SimplePaletteQuantizer.ColorCaches.Octree;
using SimplePaletteQuantizer.Extensions;
using SimplePaletteQuantizer.Helpers;

#if (UseDictionary)
using System.Collections.Concurrent;
#endif

namespace SimplePaletteQuantizer.Quantizers.DistinctSelection
{
    public class NESQuantizer : BaseColorCacheQuantizer
    {
        #region | Fields |

        private List<Color> palette;

#if (UseDictionary)
        private ConcurrentDictionary<Int32, DistinctColorInfo> colorMap;
#else
        private DistinctBucket rootBucket;
#endif

        #endregion

        #region | Methods |

        private static Boolean ProcessList(Int32 colorCount, List<DistinctColorInfo> list, ICollection<IEqualityComparer<DistinctColorInfo>> comparers, out List<DistinctColorInfo> outputList)
        {
            IEqualityComparer<DistinctColorInfo> bestComparer = null;
            Int32 maximalCount = 0;
            outputList = list;

            foreach (IEqualityComparer<DistinctColorInfo> comparer in comparers)
            {
                List<DistinctColorInfo> filteredList = list.
                    Distinct(comparer).
                    ToList();

                Int32 filteredListCount = filteredList.Count;

                if (filteredListCount > colorCount && filteredListCount > maximalCount)
                {
                    maximalCount = filteredListCount;
                    bestComparer = comparer;
                    outputList = filteredList;

                    if (maximalCount <= colorCount) break;
                }
            }

            comparers.Remove(bestComparer);
            return comparers.Count > 0 && maximalCount > colorCount;
        }

        #endregion

        #region << BaseColorCacheQuantizer >>

        /// <summary>
        /// See <see cref="IColorQuantizer.Prepare"/> for more details.
        /// </summary>
        protected override void OnPrepare(ImageBuffer image)
        {
            base.OnPrepare(image);

            OnFinish();
        }

        /// <summary>
        /// See <see cref="BaseColorCacheQuantizer.OnCreateDefaultCache"/> for more details.
        /// </summary>
        protected override IColorCache OnCreateDefaultCache()
        {
            // use OctreeColorCache best performance/quality
            return new OctreeColorCache();
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnAddColor"/> for more details.
        /// </summary>
        protected override void OnAddColor(Color color, Int32 key, Int32 x, Int32 y)
        {
#if (UseDictionary)
            colorMap.AddOrUpdate(key,
                colorKey => new DistinctColorInfo(color),
                (colorKey, colorInfo) => colorInfo.IncreaseCount());
#else
            color = QuantizationHelper.ConvertAlpha(color);
            rootBucket.StoreColor(color);
#endif
        }

        /// <summary>
        /// See <see cref="BaseColorCacheQuantizer.OnGetPaletteToCache"/> for more details.
        /// </summary>
        protected override List<Color> OnGetPaletteToCache(Int32 colorCount)
        {
            Color[] colors = {
                Color.FromArgb(101,101,101), Color.FromArgb(0,45,105), Color.FromArgb(19,31,127), Color.FromArgb(60,19,124), Color.FromArgb(96,11,98),
                Color.FromArgb(115,10,55), Color.FromArgb(113,15,7), Color.FromArgb(90,26,0), Color.FromArgb(52,40,0), Color.FromArgb(11,52,0),
                Color.FromArgb(0,60,0), Color.FromArgb(0,61,16), Color.FromArgb(0,56,64), Color.FromArgb(0,0,0), Color.FromArgb(0,0,0),
                Color.FromArgb(0,0,0), Color.FromArgb(174,174,174), Color.FromArgb(15,99,179), Color.FromArgb(64,81,208), Color.FromArgb(120,65,204),
                Color.FromArgb(167,54,169), Color.FromArgb(192,52,112), Color.FromArgb(189,60,48), Color.FromArgb(159,74,0), Color.FromArgb(109,92,0),
                Color.FromArgb(54,109,0), Color.FromArgb(7,119,4), Color.FromArgb(0,121,61), Color.FromArgb(0,114,125), Color.FromArgb(0,0,0),
                Color.FromArgb(0,0,0), Color.FromArgb(0,0,0), Color.FromArgb(254,254,255), Color.FromArgb(93,179,255), Color.FromArgb(143,161,255),
                Color.FromArgb(200,144,255), Color.FromArgb(247,133,250), Color.FromArgb(255,131,192), Color.FromArgb(255,139,127), Color.FromArgb(239,154,73),
                Color.FromArgb(189,172,44), Color.FromArgb(133,188,47), Color.FromArgb(85,199,83), Color.FromArgb(60,201,140), Color.FromArgb(62,194,205),
                Color.FromArgb(78,78,78), Color.FromArgb(0,0,0), Color.FromArgb(0,0,0), Color.FromArgb(254,254,255), Color.FromArgb(188,223,255),
                Color.FromArgb(209,216,255), Color.FromArgb(232,209,255), Color.FromArgb(251,205,253), Color.FromArgb(255,204,229), Color.FromArgb(255,207,202),
                Color.FromArgb(248,213,180), Color.FromArgb(228,220,168), Color.FromArgb(204,227,169), Color.FromArgb(185,232,184), Color.FromArgb(174,232,208),
                Color.FromArgb(175,229,234), Color.FromArgb(182,182,182), Color.FromArgb(0,0,0), Color.FromArgb(0,0,0)
            };

            palette.AddRange(colors);

            // returns our new palette
            return palette;
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.GetColorCount"/> for more details.
        /// </summary>
        protected override int OnGetColorCount()
        {
            return 64;
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnFinish"/> for more details.
        /// </summary>
        protected override void OnFinish()
        {
            base.OnFinish();

            palette = new List<Color>();

#if (UseDictionary)
            colorMap = new ConcurrentDictionary<Int32, DistinctColorInfo>();
#else
            rootBucket = new DistinctBucket();
#endif
        }

        #endregion

        #region << IColorQuantizer >>

        /// <summary>
        /// See <see cref="IColorQuantizer.AllowParallel"/> for more details.
        /// </summary>
        public override Boolean AllowParallel
        {
            get { return true; }
        }

        #endregion

        #region | Helper classes (comparers) |

        /// <summary>
        /// Compares a hue components of a color info.
        /// </summary>
        private class ColorHueComparer : IEqualityComparer<DistinctColorInfo>
        {
            public Boolean Equals(DistinctColorInfo x, DistinctColorInfo y)
            {
                return x.Hue == y.Hue;
            }

            public Int32 GetHashCode(DistinctColorInfo colorInfo)
            {
                return colorInfo.Hue.GetHashCode();
            }
        }

        /// <summary>
        /// Compares a saturation components of a color info.
        /// </summary>
        private class ColorSaturationComparer : IEqualityComparer<DistinctColorInfo>
        {
            public Boolean Equals(DistinctColorInfo x, DistinctColorInfo y)
            {
                return x.Saturation == y.Saturation;
            }

            public Int32 GetHashCode(DistinctColorInfo colorInfo)
            {
                return colorInfo.Saturation.GetHashCode();
            }
        }

        /// <summary>
        /// Compares a brightness components of a color info.
        /// </summary>
        private class ColorBrightnessComparer : IEqualityComparer<DistinctColorInfo>
        {
            public Boolean Equals(DistinctColorInfo x, DistinctColorInfo y)
            {
                return x.Brightness == y.Brightness;
            }

            public Int32 GetHashCode(DistinctColorInfo colorInfo)
            {
                return colorInfo.Brightness.GetHashCode();
            }
        }

        #endregion
    }
}
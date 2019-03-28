﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using SimplePaletteQuantizer.Helpers;
using SimplePaletteQuantizer.PathProviders;

namespace SimplePaletteQuantizer.Quantizers
{
    public abstract class BaseColorQuantizer : IColorQuantizer
    {
        #region | Constants |

        /// <summary>
        /// This index will represent invalid palette index.
        /// </summary>
        protected const Int32 InvalidIndex = -1;

        #endregion

        #region | Fields |

        private Boolean paletteFound;
        private Int64 uniqueColorIndex;
        private IPathProvider pathProvider;
        protected readonly ConcurrentDictionary<Int32, Int16> UniqueColors;

        #endregion

        #region | Constructors |

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseColorQuantizer"/> class.
        /// </summary>
        protected BaseColorQuantizer()
        {
            pathProvider = null;
            uniqueColorIndex = -1;
            UniqueColors = new ConcurrentDictionary<Int32, Int16>();
        }

        #endregion

        #region | Methods |

        /// <summary>
        /// Changes the path provider.
        /// </summary>
        /// <param name="pathProvider">The path provider.</param>
        public void ChangePathProvider(IPathProvider pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        #endregion

        #region | Helper methods |

        private IPathProvider GetPathProvider()
        {
            // if there is no path provider, it attempts to create a default one; integrated in the quantizer
            IPathProvider result = pathProvider ?? (pathProvider = OnCreateDefaultPathProvider());

            // if the provider exists; or default one was created for these purposes.. use it
            if (result == null)
            {
                String message = string.Format("The path provider is not initialized! Please use SetPathProvider() method on quantizer.");
                throw new ArgumentNullException(message);
            }

            // provider was obtained somehow, use it
            return result;
        }

        #endregion

        #region | Abstract/virtual methods |

        /// <summary>
        /// Called when quantizer is about to be prepared for next round.
        /// </summary>
        protected virtual void OnPrepare(ImageBuffer image)
        {
            uniqueColorIndex = -1;
            paletteFound = false;
            UniqueColors.Clear();
        }

        /// <summary>
        /// Called when color is to be added.
        /// </summary>
        protected virtual void OnAddColor(Color color, Int32 key, Int32 x, Int32 y)
        {
            UniqueColors.AddOrUpdate(key,
                colorKey => (Byte)Interlocked.Increment(ref uniqueColorIndex),
                (colorKey, colorIndex) => colorIndex);
        }

        /// <summary>
        /// Called when a need to create default path provider arisen.
        /// </summary>
        protected virtual IPathProvider OnCreateDefaultPathProvider()
        {
            pathProvider = new StandardPathProvider();
            return new StandardPathProvider();
        }

        /// <summary>
        /// Called when quantized palette is needed.
        /// </summary>
        protected virtual List<Color> OnGetPalette(Int32 colorCount)
        {
            // early optimalization, in case the color count is lower than total unique color count
            if (UniqueColors.Count > 0 && colorCount >= UniqueColors.Count)
            {
                // palette was found
                paletteFound = true;

                // generates the palette from unique numbers
                return UniqueColors.
                    OrderBy(pair => pair.Value).
                    Select(pair => Color.FromArgb(pair.Key)).
                    Select(color => Color.FromArgb(255, color.R, color.G, color.B)).
                    ToList();
            }

            // otherwise make it descendant responsibility
            return null;
        }

        /// <summary>
        /// Called when get palette index for a given color should be returned.
        /// </summary>
        protected virtual void OnGetPaletteIndex(Color color, Int32 key, Int32 x, Int32 y, out Int32 paletteIndex)
        {
            // by default unknown index is returned
            paletteIndex = InvalidIndex;
            Int16 foundIndex;

            // if we previously found palette quickly (without quantization), use it
            if (paletteFound && UniqueColors.TryGetValue(key, out foundIndex))
            {
                paletteIndex = foundIndex;
            }
        }

        /// <summary>
        /// Called when get color count.
        /// </summary>
        protected virtual Int32 OnGetColorCount()
        {
            return UniqueColors.Count;
        }

        /// <summary>
        /// Called when about to clear left-overs after quantization.
        /// </summary>
        protected virtual void OnFinish()
        {
            // do nothing here
        }

        #endregion

        #region << IPathProvider >>

        /// <summary>
        /// See <see cref="IPathProvider.GetPointPath"/> for more details.
        /// </summary>
        public IList<Point> GetPointPath(Int32 width, Int32 heigth)
        {
            return GetPathProvider().GetPointPath(width, heigth);
        }

        #endregion

        #region << IColorQuantizer >>

        /// <summary>
        /// See <see cref="IColorQuantizer.AllowParallel"/> for more details.
        /// </summary>
        public abstract Boolean AllowParallel { get; }

        /// <summary>
        /// See <see cref="IColorQuantizer.Prepare"/> for more details.
        /// </summary>
        public void Prepare(ImageBuffer image)
        {
            OnPrepare(image);
        }

        /// <summary>
        /// See <see cref="IColorQuantizer.AddColor"/> for more details.
        /// </summary>
        public void AddColor(Color color, Int32 x, Int32 y)
        {
            Int32 key;
            color = QuantizationHelper.ConvertAlpha(color, out key);
            OnAddColor(color, key, x, y);
        }

        /// <summary>
        /// See <see cref="IColorQuantizer.GetColorCount"/> for more details.
        /// </summary>
        public Int32 GetColorCount()
        {
            return OnGetColorCount();
        }

        /// <summary>
        /// See <see cref="IColorQuantizer.GetPalette"/> for more details.
        /// </summary>
        public List<Color> GetPalette(Int32 colorCount)
        {
            return OnGetPalette(colorCount);
        }

        /// <summary>
        /// See <see cref="IColorQuantizer.GetPaletteIndex"/> for more details.
        /// </summary>
        public Int32 GetPaletteIndex(Color color, Int32 x, Int32 y)
        {
            Int32 result, key;
            color = QuantizationHelper.ConvertAlpha(color, out key);
            OnGetPaletteIndex(color, key, x, y, out result);
            return result;
        }

        /// <summary>
        /// See <see cref="IColorQuantizer.Finish"/> for more details.
        /// </summary>
        public void Finish()
        {
            OnFinish();
        }

        #endregion
    }
}

using SimplePaletteQuantizer.ColorCaches;
using SimplePaletteQuantizer.ColorCaches.Common;
using SimplePaletteQuantizer.ColorCaches.EuclideanDistance;
using SimplePaletteQuantizer.ColorCaches.LocalitySensitiveHash;
using SimplePaletteQuantizer.ColorCaches.Octree;
using SimplePaletteQuantizer.Ditherers;
using SimplePaletteQuantizer.Ditherers.ErrorDiffusion;
using SimplePaletteQuantizer.Ditherers.Ordered;
using SimplePaletteQuantizer.Helpers;
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Quantizers.DistinctSelection;
using SimplePaletteQuantizer.Quantizers.MedianCut;
using SimplePaletteQuantizer.Quantizers.NeuQuant;
using SimplePaletteQuantizer.Quantizers.Octree;
using SimplePaletteQuantizer.Quantizers.OptimalPalette;
using SimplePaletteQuantizer.Quantizers.Popularity;
using SimplePaletteQuantizer.Quantizers.Uniform;
using SimplePaletteQuantizer.Quantizers.XiaolinWu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace NESTool.Utils
{
    public class PaletteQuantizer
    {
        public enum EColor
        {
            Color2 = 0,
            Color4 = 1,
            Color8 = 2,
            Color16 = 3,
            Color32 = 4,
            Color64 = 5,
            Color128 = 6,
            Color256 = 7
        }

        public enum EParallel
        {
            Parallel1 = 0,
            Parallel2 = 1,
            Parallel4 = 2,
            Parallel8 = 3,
            Parallel16 = 4,
            Parallel32 = 5,
            Parallel64 = 6
        }

        public enum EMethod
        {
            HSLDistictSelection = 0,
            UniformQuantization = 1,
            PopularityAlgorithm = 2,
            MedianCutAlgorithm = 3,
            OctreeQuantization = 4,
            WusColorQuantizer = 5,
            NeuQuantQuantizer = 6,
            OptimalPalette = 7,
            NESQuantizer = 8
        }

        public enum EDitherer
        {
            None = 0,
            BayerDitherer4 = 1,
            BayerDitherer8 = 2,
            ClusteredDotDitherer = 3,
            DotHalfToneDitherer = 4,
            FanDitherer = 5,
            ShiauDitherer = 6,
            SierraDitherer = 7,
            StuckiDitherer = 8,
            BurkesDitherer = 9,
            AtkinsonDithering = 10,
            TwoRowSierraDitherer = 11,
            FloydSteinbergDitherer = 12,
            JarvisJudiceNinkeDitherer = 13
        }

        public enum EColorCache
        {
            EuclideanDistance = 0,
            LocalitySensitiveHash = 1,
            OctreeSearch = 2
        }

        public enum EColorModel
        {
            RGB = 0
        }

        private Image sourceImage;
        private Image targetImage;

        private ColorModel activeColorModel;
        private IColorCache activeColorCache;
        private IColorDitherer activeDitherer;
        private IColorQuantizer activeQuantizer;

        private readonly List<ColorModel> colorModelList = new List<ColorModel>
            {
                ColorModel.RedGreenBlue,
                ColorModel.LabColorSpace,
            };

        private readonly List<IColorCache> colorCacheList = new List<IColorCache>
            {
                new EuclideanDistanceColorCache(),
                new LshColorCache(),
                new OctreeColorCache()
            };

        private readonly List<IColorDitherer> dithererList = new List<IColorDitherer>
            {
                null,
                new BayerDitherer4(),
                new BayerDitherer8(),
                new ClusteredDotDitherer(),
                new DotHalfToneDitherer(),
                new FanDitherer(),
                new ShiauDitherer(),
                new SierraDitherer(),
                new StuckiDitherer(),
                new BurkesDitherer(),
                new AtkinsonDithering(),
                new TwoRowSierraDitherer(),
                new FloydSteinbergDitherer(),
                new JarvisJudiceNinkeDitherer()
            };

        private readonly List<IColorQuantizer> quantizerList = new List<IColorQuantizer>
            {
                new DistinctSelectionQuantizer(),
                new UniformQuantizer(),
                new PopularityQuantizer(),
                new MedianCutQuantizer(),
                new OctreeQuantizer(),
                new WuColorQuantizer(),
                new NeuralColorQuantizer(),
                new OptimalPaletteQuantizer(),
                new NESQuantizer()
            };

        public string InputFileName { get; set; }
        public EColor ColorCount { get; set; } = EColor.Color256;
        public EParallel Parallel { get; set; } = EParallel.Parallel8;
        public EMethod Method { get; set; } = EMethod.HSLDistictSelection;
        public EDitherer Ditherer { get; set; } = EDitherer.None;
        public EColorCache ColorCache { get; set; } = EColorCache.EuclideanDistance;
        public EColorModel ColourModel { get; set; } = EColorModel.RGB;

        public async Task<Image> Convert()
        {
            ChangeDitherer();
            ChangeQuantizer();
            ChangeColorCache();
            ChangeColorModel();

            // tries to retrieve an image based on HSB quantization
            int parallelTaskCount = activeQuantizer.AllowParallel ? GetParallelCount() : 1;
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            int colorCount = GetColorCount();

            // disables all the controls and starts running
            sourceImage = Image.FromFile(InputFileName);

            // quantization process
            Task quantization = Task.Factory.StartNew(() =>
                targetImage = ImageBuffer.QuantizeImage(sourceImage, activeQuantizer, activeDitherer, colorCount, parallelTaskCount),
                TaskCreationOptions.LongRunning);

            // finishes after running
            await quantization.ContinueWith(task =>
            {
                // detects error and color count
                int originalColorCount = activeQuantizer.GetColorCount();

                // retrieves a BMP image based on our HSB-quantized one
                GetConvertedImage(targetImage, ImageFormat.Bmp, out int newBmpSize);

            }, uiScheduler);

            return targetImage;
        }

        private int GetColorCount()
        {
            switch (ColorCount)
            {
                case EColor.Color2: return 2;
                case EColor.Color4: return 4;
                case EColor.Color8: return 8;
                case EColor.Color16: return 16;
                case EColor.Color32: return 32;
                case EColor.Color64: return 64;
                case EColor.Color128: return 128;
                case EColor.Color256: return 256;
                default: throw new NotSupportedException("Only 2, 4, 8, 16, 32, 64, 128 and 256 colors are supported.");
            }
        }

        private int GetParallelCount()
        {
            switch (Parallel)
            {
                case EParallel.Parallel1: return 1;
                case EParallel.Parallel2: return 2;
                case EParallel.Parallel4: return 4;
                case EParallel.Parallel8: return 8;
                case EParallel.Parallel16: return 16;
                case EParallel.Parallel32: return 32;
                case EParallel.Parallel64: return 64;
                default: throw new NotSupportedException("Value not supported.");
            }
        }

        private void ChangeQuantizer()
        {
            activeQuantizer = quantizerList[(int)Method];

            // applies current UI selection
            if (activeQuantizer is BaseColorCacheQuantizer)
            {
                BaseColorCacheQuantizer quantizer = (BaseColorCacheQuantizer)activeQuantizer;
                quantizer.ChangeCacheProvider(activeColorCache);
            }

            if (Method == EMethod.UniformQuantization ||
                Method == EMethod.NeuQuantQuantizer ||
                Method == EMethod.OptimalPalette)
            {
                ColorCount = EColor.Color256;
            }

            if (Method == EMethod.NESQuantizer)
            {
                ColorCount = EColor.Color64;
            }
        }

        private void ChangeDitherer()
        {
            activeDitherer = dithererList[(int)Ditherer];
        }

        private void ChangeColorCache()
        {
            activeColorCache = colorCacheList[(int)ColorCache];

            // applies current UI selection
            if (activeQuantizer is BaseColorCacheQuantizer quantizer)
            {
                quantizer.ChangeCacheProvider(activeColorCache);
            }

            // applies current UI selection
            if (activeColorCache is BaseColorCache colorCache)
            {
                colorCache.ChangeColorModel(activeColorModel);
            }
        }

        private void ChangeColorModel()
        {
            activeColorModel = colorModelList[(int)ColourModel];

            // applies current UI selection
            if (activeColorCache is BaseColorCache colorCache)
            {
                colorCache.ChangeColorModel(activeColorModel);
            }
        }

        private static Image GetConvertedImage(Image image, ImageFormat newFormat, out int imageSize)
        {
            Image result;

            // saves the image to the stream, and then reloads it as a new image format; thus conversion.. kind of
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, newFormat);
                stream.Seek(0, SeekOrigin.Begin);
                imageSize = (int)stream.Length;
                result = Image.FromStream(stream);
            }

            return result;
        }
    }
}

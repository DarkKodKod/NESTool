﻿using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using NESTool.UserControls.Views;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class CharacterViewModel : ItemViewModel
    {
        private ObservableCollection<ActionTabItem> _tabs;
        private bool _doNotSavePalettes = false;
        private PaletteIndex _paletteIndex = 0;
        private FileModelVO[] _palettes;
        private int _selectedPalette1 = -1;
        private int _selectedPalette2 = -1;
        private int _selectedPalette3 = -1;
        private int _selectedPalette4 = -1;

        public static Dictionary<string, WriteableBitmap> FrameBitmapCache;
        public static Dictionary<Tuple<int, int>, Dictionary<Color, Color>> GroupedPalettes;

        #region Commands
        public CharacterCloseTabCommand CharacterCloseTabCommand { get; } = new CharacterCloseTabCommand();
        public CharacterNewTabCommand CharacterNewTabCommand { get; } = new CharacterNewTabCommand();
        #endregion

        public CharacterModel GetModel()
        {
            if (ProjectItem?.FileHandler.FileModel is CharacterModel model)
            {
                return model;
            }

            return null;
        }

        #region get/set
        public ObservableCollection<ActionTabItem> Tabs
        {
            get
            {
                if (_tabs == null)
                {
                    _tabs = new ObservableCollection<ActionTabItem>();
                    IEditableCollectionView itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_tabs);
                    itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
                }

                return _tabs;
            }
        }

        public int SelectedPalette1
        {
            get { return _selectedPalette1; }
            set
            {
                _selectedPalette1 = value;

                OnPropertyChanged("SelectedPalette1");
            }
        }

        public int SelectedPalette2
        {
            get { return _selectedPalette2; }
            set
            {
                _selectedPalette2 = value;

                OnPropertyChanged("SelectedPalette2");
            }
        }

        public int SelectedPalette3
        {
            get { return _selectedPalette3; }
            set
            {
                _selectedPalette3 = value;

                OnPropertyChanged("SelectedPalette3");
            }
        }

        public int SelectedPalette4
        {
            get { return _selectedPalette4; }
            set
            {
                _selectedPalette4 = value;

                OnPropertyChanged("SelectedPalette4");
            }
        }

        public FileModelVO[] Palettes
        {
            get { return _palettes; }
            set
            {
                _palettes = value;

                OnPropertyChanged("Palettes");
            }
        }

        public PaletteIndex PaletteIndex
        {
            get { return _paletteIndex; }
            set
            {
                if (_paletteIndex != value)
                {
                    _paletteIndex = value;

                    foreach (ActionTabItem tab in Tabs)
                    {
                        if (tab.Content is CharacterFrameEditorView frameView)
                        {
                            if (frameView.DataContext is CharacterFrameEditorViewModel viewmodel)
                            {
                                if (viewmodel.IsActive)
                                {
                                    viewmodel.SaveProperty(SpriteProperties.PaletteIndex, new ValueUnion { integer = (int)value });
                                }
                            }
                        }
                    }
                }

                OnPropertyChanged("PaletteIndex");
            }
        }
        #endregion

        public override void OnActivate()
        {
            base.OnActivate();

            List<FileModelVO> list = new List<FileModelVO>
            {
                new FileModelVO()
                {
                    Index = -1,
                    Name = "None",
                    Model = null
                }
            };

            list.AddRange(ProjectFiles.GetModels<PaletteModel>());

            Palettes = list.ToArray();

            FrameBitmapCache = new Dictionary<string, WriteableBitmap>();
            GroupedPalettes = new Dictionary<Tuple<int, int>, Dictionary<Color, Color>>();

            #region Signals
            SignalManager.Get<AnimationTabDeletedSignal>().AddListener(OnAnimationTabDeleted);
            SignalManager.Get<AnimationTabNewSignal>().AddListener(OnAnimationTabNew);
            SignalManager.Get<RenamedAnimationTabSignal>().AddListener(OnRenamedAnimationTab);
            SignalManager.Get<SwitchCharacterFrameViewSignal>().AddListener(OnSwitchCharacterFrameView);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().AddListener(OnColorPaletteControlSelected);
            SignalManager.Get<UpdateCharacterImageSignal>().AddListener(OnUpdateCharacterImage);
            SignalManager.Get<SelectPaletteIndexSignal>().AddListener(OnSelectPaletteIndex);
            #endregion

            PopulateTabs();

            foreach (ActionTabItem tab in Tabs)
            {
                if (tab.Content.DataContext is AActivate vm)
                {
                    vm.OnActivate();
                }
            }

            LoadPalettes();
        }

        private void LoadPalettes()
        {
            _doNotSavePalettes = true;

            byte R;
            byte G;
            byte B;

            // Load palettes
            for (int i = 0; i < 4; ++i)
            {
                string paletteId = GetModel().PaletteIDs[i];

                PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);
                if (paletteModel == null)
                {
                    continue;
                }

                R = (byte)(paletteModel.Color0 >> 16);
                G = (byte)(paletteModel.Color0 >> 8);
                B = (byte)paletteModel.Color0;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 0);

                R = (byte)(paletteModel.Color1 >> 16);
                G = (byte)(paletteModel.Color1 >> 8);
                B = (byte)paletteModel.Color1;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 1);

                R = (byte)(paletteModel.Color2 >> 16);
                G = (byte)(paletteModel.Color2 >> 8);
                B = (byte)paletteModel.Color2;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 2);

                R = (byte)(paletteModel.Color3 >> 16);
                G = (byte)(paletteModel.Color3 >> 8);
                B = (byte)paletteModel.Color3;

                SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(Color.FromRgb(R, G, B), i, 3);
            }

            _doNotSavePalettes = false;
        }

        private void OnColorPaletteControlSelected(Color color, int paletteIndex, int colorPosition)
        {
            if (!IsActive)
            {
                return;
            }

            if (_doNotSavePalettes)
            {
                return;
            }

            string paletteId = GetModel().PaletteIDs[paletteIndex];

            PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);
            if (paletteModel == null)
            {
                return;
            }

            int colorInt = ((color.R & 0xff) << 16) | ((color.G & 0xff) << 8) | (color.B & 0xff);

            int prevColorInt = 0;

            switch (colorPosition)
            {
                case 0:
                    prevColorInt = paletteModel.Color0;
                    paletteModel.Color0 = colorInt;
                    break;
                case 1:
                    prevColorInt = paletteModel.Color1;
                    paletteModel.Color1 = colorInt;
                    break;
                case 2:
                    prevColorInt = paletteModel.Color2;
                    paletteModel.Color2 = colorInt;
                    break;
                case 3:
                    prevColorInt = paletteModel.Color3;
                    paletteModel.Color3 = colorInt;
                    break;
            }

            ProjectItem.FileHandler.Save();

            // Convert previous color to type Color
            byte R = (byte)(prevColorInt >> 16);
            byte G = (byte)(prevColorInt >> 8);
            byte B = (byte)prevColorInt;

            Color prevColor = Color.FromRgb(R, G, B);

            AdjustPaletteCache(paletteIndex, colorPosition, prevColor, color);

            SignalManager.Get<UpdateCharacterImageSignal>().Dispatch();
        }

        private void AdjustPaletteCache(int paletteIndex, int colorPosition, Color prevColor, Color color)
        {
            foreach (KeyValuePair<Tuple<int, int>, Dictionary<Color, Color>> entry in GroupedPalettes)
            {
                Tuple<int, int> tuple = entry.Key as Tuple<int, int>;

                if (tuple.Item2 == paletteIndex)
                {
                    int index = 0;
                    foreach (KeyValuePair<Color, Color> entry2 in entry.Value)
                    {
                        if (index == colorPosition && entry2.Value == prevColor)
                        {
                            entry.Value[entry2.Key] = color;
                            break;
                        }

                        index++;
                    }
                }
            }
        }

        private void OnUpdateCharacterImage()
        {
            foreach (ActionTabItem tab in Tabs)
            {
                if (tab.FramesView is CharacterAnimationView frameView)
                {
                    if (frameView.DataContext is CharacterAnimationViewModel viewModel)
                    {
                        viewModel.LoadFrameImage();
                    }

                    foreach (CharacterFrameView frame in frameView.FrameViewList)
                    {
                        frame.OnActivate();
                    }
                }
            }
        }

        private void OnSelectPaletteIndex(PaletteIndex paletteIndex)
        {
            PaletteIndex = paletteIndex;
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<AnimationTabDeletedSignal>().RemoveListener(OnAnimationTabDeleted);
            SignalManager.Get<AnimationTabNewSignal>().RemoveListener(OnAnimationTabNew);
            SignalManager.Get<RenamedAnimationTabSignal>().RemoveListener(OnRenamedAnimationTab);
            SignalManager.Get<SwitchCharacterFrameViewSignal>().RemoveListener(OnSwitchCharacterFrameView);
            SignalManager.Get<ColorPaletteControlSelectedSignal>().RemoveListener(OnColorPaletteControlSelected);
            SignalManager.Get<UpdateCharacterImageSignal>().RemoveListener(OnUpdateCharacterImage);
            SignalManager.Get<SelectPaletteIndexSignal>().RemoveListener(OnSelectPaletteIndex);
            #endregion

            foreach (ActionTabItem tab in Tabs)
            {
                if (tab.Content.DataContext is AActivate vm)
                {
                    vm.OnDeactivate();
                }
            }
        }

        public void PopulateTabs()
        {
            CharacterModel model = GetModel();

            if (model == null)
            {
                return;
            }

            foreach (CharacterAnimation animation in model.Animations)
            {
                if (string.IsNullOrEmpty(animation.ID))
                {
                    continue;
                }

                AddNewAnimation(animation.ID, animation.Name);
            }
        }

        private void OnRenamedAnimationTab(string newName)
        {
            if (!IsActive)
            {
                return;
            }

            Save();
        }

        private void OnAnimationTabNew()
        {
            if (!IsActive)
            {
                return;
            }

            string newTabName = "Animation_" + (Tabs.Count + 1);

            AddNewAnimation(Guid.NewGuid().ToString(), newTabName);

            Save();
        }

        private void AddNewAnimation(string id, string animationName)
        {
            CharacterAnimationView animationView = new CharacterAnimationView();
            ((CharacterAnimationViewModel)animationView.DataContext).CharacterModel = GetModel();
            ((CharacterAnimationViewModel)animationView.DataContext).FileHandler = ProjectItem.FileHandler;
            ((CharacterAnimationViewModel)animationView.DataContext).TabID = id;

            CharacterFrameEditorView frameView = new CharacterFrameEditorView();
            ((CharacterFrameEditorViewModel)frameView.DataContext).CharacterModel = GetModel();
            ((CharacterFrameEditorViewModel)frameView.DataContext).FileHandler = ProjectItem.FileHandler;
            ((CharacterFrameEditorViewModel)frameView.DataContext).TabID = id;

            Tabs.Add(new ActionTabItem
            {
                ID = id,
                Header = animationName,
                Content = animationView,
                FramesView = animationView,
                PixelsView = frameView
            });
        }

        private void OnSwitchCharacterFrameView(string tabId, int frameIndex)
        {
            if (!IsActive)
            {
                return;
            }

            foreach (ActionTabItem tab in Tabs)
            {
                if (tab.ID == tabId)
                {
                    tab.SwapContent(tabId, frameIndex);

                    if (tab.Content is CharacterAnimationView frameView)
                    {
                        foreach (CharacterFrameView frame in frameView.FrameViewList)
                        {
                            frame.OnActivate();
                        }
                    }

                    return;
                }
            }
        }

        private void Save()
        {
            CharacterModel model = GetModel();

            if (model != null)
            {
                int index = 0;

                foreach (ActionTabItem tab in Tabs)
                {
                    CharacterAnimationView view = tab.FramesView as CharacterAnimationView;
                    CharacterAnimationViewModel viewModel = view.DataContext as CharacterAnimationViewModel;

                    model.Animations[index].ID = tab.ID;
                    model.Animations[index].Name = tab.Header;
                    model.Animations[index].Speed = viewModel.Speed;

                    index++;
                }

                for (int i = index; i < model.Animations.Length; ++i)
                {
                    model.Animations[i].ID = string.Empty;
                }

                ProjectItem.FileHandler.Save();
            }
        }

        private void OnAnimationTabDeleted(ActionTabItem tabItem)
        {
            if (!IsActive)
            {
                return;
            }

            foreach (ActionTabItem tab in Tabs)
            {
                if (tab == tabItem)
                {
                    Tabs.Remove(tab);

                    Save();

                    return;
                }
            }
        }
    }
}

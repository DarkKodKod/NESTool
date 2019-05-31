using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using NESTool.UserControls.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace NESTool.ViewModels
{
    public class CharacterViewModel : ItemViewModel
    {
        private ObservableCollection<ActionTabItem> _tabs;

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
                    var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_tabs);
                    itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
                }

                return _tabs;
            }
        }
        #endregion

        public CharacterViewModel()
        {
            #region Signals
            SignalManager.Get<AnimationTabDeletedSignal>().AddListener(OnAnimationTabDeleted);
            SignalManager.Get<AnimationTabNewSignal>().AddListener(OnAnimationTabNew);
            SignalManager.Get<RenamedAnimationTabSignal>().AddListener(OnRenamedAnimationTab);
            SignalManager.Get<SwitchCharacterFrameViewSignal>().AddListener(OnSwitchCharacterFrameView);
            #endregion
        }

        public override void OnActivate()
        {
            base.OnActivate();

            PopulateTabs();

            foreach (ActionTabItem tab in Tabs)
            {
                if (tab.Content.DataContext is AActivate vm)
                {
                    vm.OnActivate();
                }
            }
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

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
                    CharacterAnimationView view = tab.Content as CharacterAnimationView;
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

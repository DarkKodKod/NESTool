using NESTool.Architecture.Model;
using Nett;

namespace NESTool.Models
{
    public class ProjectModel : IModel
    {
        public int INesMapper
        {
            get { return _iNesmapper; }
            set
            {
                if (_iNesmapper != value)
                {
                    _iNesmapper = value;
                    Save();
                }
            }
        }

        public int CHRSize
        {
            get { return _chrSize; }
            set
            {
                if (_chrSize != value)
                {
                    _chrSize = value;
                    Save();
                }
            }
        }

        public int PRGSize
        {
            get { return _prgSize; }
            set
            {
                if (_prgSize != value)
                {
                    _prgSize = value;
                    Save();
                }
            }
        }

        public int WindowSizeX
        {
            get { return _windowSizeX; }
            set
            {
                if (_windowSizeX != value)
                {
                    _windowSizeX = value;
                    Save();
                }
            }
        }

        public int WindowSizeY
        {
            get { return _windowSizeY; }
            set
            {
                if (_windowSizeY != value)
                {
                    _windowSizeY = value;
                    Save();
                }
            }
        }

        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                if (_fullScreen != value)
                {
                    _fullScreen = value;
                    Save();
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    Save();
                }
            }
        }

        private int _iNesmapper;
        private int _chrSize;
        private int _prgSize;
        private bool _fullScreen;
        private int _windowSizeX;
        private int _windowSizeY;
        private string _name = "";
        private string _projectPath;

        public void Reset()
        {
            _projectPath = "";

            Name = "";
        }

        public void Copy(ProjectModel copy)
        {
            Name = copy.Name;
            WindowSizeX = copy.WindowSizeX;
            WindowSizeY = copy.WindowSizeY;
            FullScreen = copy.FullScreen;
            INesMapper = copy.INesMapper;
            CHRSize = copy.CHRSize;
            PRGSize = copy.PRGSize;
        }

        public void Load(string path)
        {
            _projectPath = path;

            Copy(Toml.ReadFile<ProjectModel>(_projectPath));
        }

        public void Save(string path)
        {
            _projectPath = path;

            Save();
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_projectPath))
            {
                return;
            }

            Toml.WriteFile(this, _projectPath);
        }
    }
}

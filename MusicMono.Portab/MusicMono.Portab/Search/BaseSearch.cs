using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicMono.Portab.SearchObjects;
using System.Threading;

namespace MusicMono.Portab.Search
{
    public abstract class BaseSearch
    {
        

        protected bool _changed;
        protected static Dictionary<string, BaseSearchObject> _history = new Dictionary<string, BaseSearchObject>();
        protected CancellationToken ct;
        protected CancellationTokenSource cs;

        private List<BaseSearchObject> _currentList;
        private List<string> _activeKeys;
        private object _lockFile = new object();
        public BaseSearchObject this[string ID]
        {
            get
            {
                if (_history.ContainsKey(ID))
                {
                    return _history[ID];
                }
                else
                {
                    return null;
                }
            }
        }
        public BaseSearchObject this[int i]
        {
            get
            {
                if (_activeKeys != null && _activeKeys.Count > i)
                {
                    return this[_activeKeys[i]];
                }
                else
                {
                    throw new Exception("There is nothing on this loc \"YoutubeSearch[i]\"");
                }
            }
        }
        public List<BaseSearchObject> CurrentList
        {
            get
            {
                if (_changed)
                {
                    _changed = false;

                    _currentList = _activeKeys != null &&
                                   _history != null &&
                                   _activeKeys.Count > 0 ? (from key in _activeKeys
                                                            where key != null
                                                            where _history.ContainsKey(key)
                                                            group _history[key] by _history[key].Type into objz
                                                            from obj in objz
                                                            select obj).ToList()  : new List<BaseSearchObject>();
                }
                return _currentList;
            }
        }
        public event EventHandler<EventArgs> OnChange;
        public BaseSearch()
        {
            _history = new Dictionary<string, BaseSearchObject>();
            _activeKeys = new List<string>();
            _changed = true;
        }
        public async Task SearchAsync(string Name)
        {
            await SearchAsync(Name, SearchObject.Any);            
        }
        public async Task SearchAsync(string Name,SearchObject SearchObject)
        {
            lock (_lockFile)
            {
                cs?.Cancel();
                cs = new CancellationTokenSource();
                ct = cs.Token;
            }
            await Task.Factory.StartNew( async() =>
            {
                try
                {
                    
                    Task del = Task.Delay(1500, ct);
                    await del;
                    if (!del.IsCanceled)
                    {
                        DeleteActiveObjects();
                        await QAsync(Name, SearchObject);
                    }
                }catch(Exception ex)
                {
                    if (ex != null)
                        ex = null;

                }
            }, ct);
        }
        public async Task NextPageAsync()
        {
            await QAsync();
        }
        protected abstract Task QAsync();
        protected abstract Task QAsync(string Name);
        protected abstract Task QAsync(string Name, SearchObject SearchObject);
        protected void Song_OnChange(object sender, EventArgs e)
        {
            _changed = true;
            OnChange?.Invoke(this, EventArgs.Empty);
        }
        protected void AddActiveSearchObjects(IEnumerable<BaseSearchObject> BaseObjects)
        {
            if (BaseObjects != null)
                if (BaseObjects.Any())
                    _activeKeys.AddRange(from Obj in BaseObjects
                                         select Obj.ID);
        }
        protected void AddActiveSearchObject(BaseSearchObject BaseObject)
        {
            if (BaseObject != null)
                _activeKeys.Add(BaseObject.ID);
        }
        protected void DeleteActiveObjects()
        {
            _activeKeys.RemoveRange(0, _activeKeys.Count);
        }

    }
}
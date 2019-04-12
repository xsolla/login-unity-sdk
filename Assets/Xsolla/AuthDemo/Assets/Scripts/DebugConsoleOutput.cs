namespace Puzzly.Utils
{

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class DebugConsoleOutput : MonoBehaviour
    {
        public bool showLogMessages = false;
        public bool showWarningMessages = true;
        public bool showErrorMessages = true;
        public bool showExceptionMessages = true;



        private class ListItem
        {
            public GUIContent Content { get; set; }
            public Rect Position { get; set; }
            public Color Color { get; set; }
        }



        private class LogItem
        {
            public DateTime Time { get; set; }
            public LogType Type { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }

            public override string ToString()
            {
                string msg = string.Format("{0} [{1}] {2}", Time.ToString("HH:mm:ss.fff"), Type.ToString().ToUpper(), Message);
                if (!string.IsNullOrEmpty(StackTrace))
                {
                    switch (Type)
                    {
                        case LogType.Exception:
                        case LogType.Error:
                            msg += "\r\n" + StackTrace;
                            break;
                    }
                }

                return msg.Trim();
            }
        }



        private bool _isOpened = false;

        private Queue<LogItem> _addQueue = new Queue<LogItem>();
        private Vector2 _scrollPosition = Vector2.zero;

        private Vector2 _lastMousePosition = Vector2.zero;
        private bool _isMousePressed = false;

        private List<ListItem> _viewItems = new List<ListItem>();





        bool _updateLayout = false;


        private Rect _containerRect;
        private Rect _scrollViewContainerRect;
        private Rect _scrollViewRect;
        private Rect _itemsViewRect;
        private Rect _showHideButtonRect;
        private Rect _scrollToEndButtonRect;
        private Rect _visibleAreaRect;


        private int _screenWidth;
        private int _screenHeight;


        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            UnityEngine.Application.logMessageReceived += HandleLog;

            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            UpdateLayout();
        }

        void Update()
        {
            if (Screen.width != _screenWidth || Screen.height != _screenHeight)
            {
                _screenWidth = Screen.width;
                _screenHeight = Screen.height;

                UpdateLayout();
            }



            if (_isOpened)
            {
                var mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                if (Input.GetMouseButtonDown(0) && _visibleAreaRect.Contains(mousePosition))
                {
                    _isMousePressed = true;
                    _lastMousePosition = mousePosition;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    _isMousePressed = false;
                }


                if (_isMousePressed && mousePosition != _lastMousePosition)
                {
                    _scrollPosition -= new Vector2(0, mousePosition.y - _lastMousePosition.y);
                    _lastMousePosition = mousePosition;

                    MarkVisibleItemsAsChanged();
                }
            }
        }

        void OnDestroy()
        {
            UnityEngine.Application.logMessageReceived -= HandleLog;
        }


        public void UpdateLayout()
        {
            _updateLayout = true;
        }

        private bool UpdateLayoutIfNeed()
        {
            if (!_updateLayout)
                return false;

            _updateLayout = false;
            UpdateLayoutInternal();

            return true;
        }

        private void UpdateLayoutInternal()
        {
            const float buttonWidth = 120;
            const float buttonHeight = 24;
            const float space = 8;

            bool horizontalOrientation = Screen.height > Screen.width;
            if (horizontalOrientation)
            {
                float x = space;
                float y = Screen.height * 0.5f;
                float width = Screen.width - x - space;
                float height = Screen.height - y - space;

                _containerRect = new Rect(x, y, width, height);
            }
            else
            {
                float x = space;
                float y = space;
                float width = Screen.width * 0.5f - x;
                float height = Screen.height - y - space;

                _containerRect = new Rect(x, y, width, height);
            }



            _scrollViewContainerRect = _containerRect;
            _scrollViewContainerRect.height -= buttonHeight + space;

            const float border = 2;
            _scrollViewRect = _scrollViewContainerRect;
            _scrollViewRect.x += border;
            _scrollViewRect.y += border;
            _scrollViewRect.width -= border * 2;
            _scrollViewRect.height -= border * 2;



            _showHideButtonRect = new Rect(20,20, buttonWidth, buttonHeight);

            _scrollToEndButtonRect = _showHideButtonRect;
            _scrollToEndButtonRect.x += _scrollToEndButtonRect.width + space;



            float scrollBarWidth = GUI.skin.verticalScrollbar.fixedWidth + 1;
            _visibleAreaRect = _scrollViewRect;
            _visibleAreaRect.width -= scrollBarWidth;

            _itemsViewRect = new Rect(0, 0, _visibleAreaRect.width, 0);



            UpdateItemsGeometryInternal();

            MarkVisibleItemsAsChanged();
        }



        private void UpdateItemsGeometryInternal()
        {
            float x = 0;
            float y = 0;
            float width = _itemsViewRect.width;

            foreach (var item in _viewItems)
            {
                float height = GeneralItemStyle.CalcHeight(item.Content, width);
                item.Position = new Rect(x, y, width, height);

                y += height;
            }

            _itemsViewRect.height = y;

            MarkVisibleItemsAsChanged();
        }







        private int FindVisibleItemIndex()
        {
            int first = 0;
            int last = _viewItems.Count - 1;

            if (first == last)
            {
                Rect rect = _viewItems[first].Position;
                rect.x += (_scrollViewRect.x - _scrollPosition.x);
                rect.y += (_scrollViewRect.y - _scrollPosition.y);

                if (_scrollViewRect.Overlaps(rect))
                    return first;

                return -1;
            }

            while (first < last)
            {
                int middle = first + (last - first) / 2;

                Rect rect = _viewItems[middle].Position;
                rect.x += (_scrollViewRect.x - _scrollPosition.x);
                rect.y += (_scrollViewRect.y - _scrollPosition.y);

                if (_scrollViewRect.Overlaps(rect))
                {
                    return middle;
                }
                else if (rect.yMin < _scrollViewRect.y)
                {
                    first = middle + 1;
                }
                else
                {
                    last = middle; // TODO: -1 (?)
                }
            }

            return -1;
        }





        private bool _isVisibleItemsChanged = false;

        private void MarkVisibleItemsAsChanged()
        {
            _isVisibleItemsChanged = true;
        }


        private List<ListItem> _cachedVisibleItems = null;

        private List<ListItem> GetVisibleItems()
        {
            if (_cachedVisibleItems != null && !_isVisibleItemsChanged)
                return _cachedVisibleItems;


            _isVisibleItemsChanged = false;
            _cachedVisibleItems = GetVisibleItemsInternal();

            return _cachedVisibleItems;
        }

        private List<ListItem> GetVisibleItemsInternal()
        {
            var items = new List<ListItem>();

            int index = FindVisibleItemIndex();
            if (index >= 0)
            {
                //UnityDebug.Log("{0}", index);

                int first = index;
                while (first >= 0)
                {
                    Rect rect = _viewItems[first].Position;
                    rect.x += (_scrollViewRect.x - _scrollPosition.x);
                    rect.y += (_scrollViewRect.y - _scrollPosition.y);

                    if (_scrollViewRect.Overlaps(rect))
                    {
                        first--;
                        if (first <= 0)
                        {
                            first = 0;
                            break;
                        }
                    }
                    else
                        break;
                }

                int last = index;
                while (last <= _viewItems.Count - 1)
                {
                    Rect rect = _viewItems[last].Position;
                    rect.x += (_scrollViewRect.x - _scrollPosition.x);
                    rect.y += (_scrollViewRect.y - _scrollPosition.y);

                    if (_scrollViewRect.Overlaps(rect))
                    {
                        last++;
                        if (last >= _viewItems.Count - 1)
                        {
                            last = _viewItems.Count - 1;
                            break;
                        }
                    }
                    else
                        break;
                }

                //UnityDebug.Log("from {0} to {1} (count = {2})", first, last, _list.Count);
                for (int i = first; i <= last; i++)
                    items.Add(_viewItems[i]);

            }
            else
            {
                //UnityDebug.Log("{0}", index);
            }

            return items;
        }




        bool IsEndOfList()
        {
            return _scrollPosition.y >= (_itemsViewRect.height - _scrollViewRect.height);
        }

        void ScrollToEndOfList()
        {
            var endPosition = new Vector2(0, _itemsViewRect.height - _scrollViewRect.height);
            if (_scrollPosition != endPosition)
            {
                _scrollPosition = endPosition;
                MarkVisibleItemsAsChanged();
            }
        }




        void AddToEndOfList(LogItem item)
        {
            string log = item.ToString();
            LogType type = item.Type;
            if (string.IsNullOrEmpty(log))
            {
                type = LogType.Warning;
                log = "Empty log message!";
            }

            var listItem = new ListItem();
            listItem.Content = new GUIContent(log);

            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    listItem.Color = Color.red;
                    break;
                case LogType.Warning:
                    listItem.Color = Color.yellow;
                    break;
                case LogType.Log:
                case LogType.Assert:
                    listItem.Color = Color.gray;
                    break;
                default:
                    listItem.Color = Color.white;
                    break;
            }


            float x = 0;
            float y = _itemsViewRect.height;
            float width = _itemsViewRect.width;
            float height = GeneralItemStyle.CalcHeight(listItem.Content, width);

            listItem.Position = new Rect(x, y, width, height);
            _viewItems.Add(listItem);



            var scrollToEnd = IsEndOfList();

            _itemsViewRect.height += listItem.Position.height;

            if (scrollToEnd)
                ScrollToEndOfList();
        }

        void ProcessItems(int max = 8)
        {
            lock (_addQueue)
            {
                int processed = 0;
                while (_addQueue.Count > 0 && processed < max)
                {
                    var item = _addQueue.Dequeue();
                    AddToEndOfList(item);
                    processed++;
                }
            }
        }




        private GUIStyle _generalItemStyle;
        protected GUIStyle GeneralItemStyle
        {
            get
            {
                if (_generalItemStyle == null)
                {
                    _generalItemStyle = new GUIStyle(GUI.skin.label);
                    _generalItemStyle.wordWrap = true;
                }

                return _generalItemStyle;
            }
        }



        private Dictionary<Color, GUIStyle> _itemStylesCache = new Dictionary<Color, GUIStyle>();

        private GUIStyle GetStyleForItem(Color color)
        {
            GUIStyle style = null;
            if (_itemStylesCache.TryGetValue(color, out style))
                return style;

            style = new GUIStyle(GeneralItemStyle);

            var backgroundColor = color;
            backgroundColor.a *= 0.25f;

            var backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, backgroundColor);
            backgroundTexture.Apply();

            var foregroundColor = color;
            //foregroundColor.a = 1.0f;
            foregroundColor.r += 0.5f;
            foregroundColor.g += 0.5f;
            foregroundColor.b += 0.5f;

            style.normal.textColor = foregroundColor;
            style.normal.background = backgroundTexture;

            _itemStylesCache.Add(color, style);

            return style;
        }




        void OnGUI()
        {
            UpdateLayoutIfNeed();

            ProcessItems();


            if (GUI.Button(_showHideButtonRect, _isOpened ? "hide console" : "show console"))
                _isOpened = !_isOpened;

            if (_isOpened)
            {
                if (GUI.Button(_scrollToEndButtonRect, "scroll to end"))
                    ScrollToEndOfList();


                GUI.Box(_scrollViewContainerRect, GUIContent.none);
                var scrollPosition = GUI.BeginScrollView(_scrollViewRect, _scrollPosition, _itemsViewRect);
                if (scrollPosition != _scrollPosition)
                {
                    _scrollPosition = scrollPosition;
                    MarkVisibleItemsAsChanged();
                }

                var visibleItems = GetVisibleItems();
                foreach (var item in visibleItems)
                {
                    var style = GetStyleForItem(item.Color);
                    GUI.Label(item.Position, item.Content, style);
                }

                GUI.EndScrollView();
            }
        }

        void HandleLog(string message, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log: if (!showLogMessages) return; break;
                case LogType.Warning: if (!showWarningMessages) return; break;
                case LogType.Error: if (!showErrorMessages) return; break;
                case LogType.Exception: if (!showExceptionMessages) return; break;
                default: return;
            }


            var item = new LogItem()
            {
                Time = DateTime.Now,
                Type = type,
                Message = message,
                StackTrace = stackTrace
            };

            lock (_addQueue)
                _addQueue.Enqueue(item);
        }
    }
}
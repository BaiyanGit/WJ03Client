using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Wx.Runtime.Debugger
{
    public class WDebugger : WModule
    {
        public override int Priority => 12;

        [Header("是否开启Debugger"),SerializeField]
        private bool debugger;
        [Header("是否输出日志"), SerializeField]
        private bool output;


        private static FileStream _fileWriter;
        private static UTF8Encoding _encoding;
        private StringBuilder _sb;

        private struct LogInfo
        {
            public readonly LogType type;
            public readonly string desc;

            public LogInfo(LogType type, string desc)
            {
                this.type = type;
                this.desc = desc;
            }
        }

        //错误详情
        private readonly List<LogInfo> _logEntries = new List<LogInfo>();
        private readonly List<LogInfo> _logLog = new List<LogInfo>();
        private readonly List<LogInfo> _logWarning = new List<LogInfo>();
        private readonly List<LogInfo> _logError = new List<LogInfo>();
        private readonly List<LogInfo> _logException = new List<LogInfo>();
        private List<LogInfo> _curLog;
        //是否显示错误窗口
        private bool _isVisible = false;
        //窗口显示区域
        private Rect _windowRect = new Rect(0, 50, Screen.width, Screen.height - 50);
        //窗口滚动区域
        private Vector2 _scrollPositionText = Vector2.zero;
        //字体大小
        private int _fontSize = 30;


        private GUISkin _guiSkin;



        protected override void Awake()
        {
            base.Awake();
            _guiSkin = Resources.Load<GUISkin>("GUI/GUI Skin");
            _guiSkin.button.fontSize = _fontSize;
            _guiSkin.textArea.fontSize = _fontSize;
#if !UNITY_EDITOR
            if (output)
            {
                InitOutput();
            }
            if (debugger)
            {
                InitDebugger();
            }
            
#endif
        }
        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
#if UNITY_EDITOR
            return;
#endif

            if (!Input.GetKey(KeyCode.S) || !Input.GetKeyDown(KeyCode.B)) return;
            debugger = !debugger;
            if (debugger)
            {
                InitDebugger();
            }
            else
            {
                Application.logMessageReceivedThreaded -= DebuggerCallBack;
                ClearLog();
            }

        }

        private void InitOutput()
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Log");
            var nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "_").Replace("/", "_").Replace(":", "_");
            FileInfo fileInfo = new(Application.persistentDataPath + "/Log/" + nowTime + "_Log.txt");
            //设置Log文件输出地址
            _fileWriter = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _encoding = new UTF8Encoding();
            _sb = new StringBuilder();
            Application.logMessageReceivedThreaded += LogCallback;
        }

        /// <summary>
        /// Log回调
        /// </summary>
        /// <param name="condition">输出内容</param>
        /// <param name="stackTrace">堆栈追踪</param>
        /// <param name="type">Log日志类型</param>
        private void LogCallback(string condition, string stackTrace, LogType type) //写入控制台数据
        {
            //输出的日志类型可以自定义
            _sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _sb.Append(" 【");
            _sb.Append(type);
            _sb.Append("】【");
            _sb.Append(stackTrace);
            _sb.Append("】:");
            _sb.Append(condition);
            _sb.Append(Environment.NewLine);
            _sb.Append(Environment.NewLine);
            var content = _sb.ToString();
            _fileWriter.Write(_encoding.GetBytes(content), 0, _encoding.GetByteCount(content));
            _fileWriter.Flush();
            _sb.Clear();
        }

        private void InitDebugger()
        {
            Application.logMessageReceivedThreaded += DebuggerCallBack;
            _curLog = _logEntries;
        }
        
        private void ClearLog()
        {
            _logEntries.Clear();
            _logLog.Clear();
            _logWarning.Clear();
            _logException.Clear();
            _logError.Clear();
        }

        private void CheckLogListType(List<LogInfo> list)
        {
            _curLog = _curLog == list ? _logEntries : list;
        }
        
        private void DebuggerCallBack(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Warning:
                    _logWarning.Add(new LogInfo(type, $"{condition}\n{stackTrace}"));
                    break;
                case LogType.Log:
                    _logLog.Add(new LogInfo(type, $"{condition}\n{stackTrace}"));
                    break;
                case LogType.Error:
                    _logError.Add(new LogInfo(type, $"{condition}\n{stackTrace}"));
                    break;
                case LogType.Exception:
                    _logException.Add(new LogInfo(type, $"{condition}\n{stackTrace}"));
                    break;
                case LogType.Assert:
                    break;
                default:
                    break;
            }
            _logEntries.Add(new LogInfo(type, $"{condition}\n{stackTrace}"));
        }

        private void ConsoleWindow(int windowID)
        {
            _guiSkin.button.fontSize = _fontSize;
            _guiSkin.textArea.fontSize = _fontSize;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Log", _guiSkin.button, GUILayout.MaxWidth(200), GUILayout.MaxHeight(100)))
            {
                CheckLogListType(_logLog);
            }
            if (GUILayout.Button("Warning", _guiSkin.button, GUILayout.MaxWidth(200), GUILayout.MaxHeight(100)))
            {
                CheckLogListType(_logWarning);
            }
            if (GUILayout.Button("Error", _guiSkin.button, GUILayout.MaxWidth(200), GUILayout.MaxHeight(100)))
            {
                CheckLogListType(_logError);
            }
            if (GUILayout.Button("Exception", _guiSkin.button, GUILayout.MaxWidth(200), GUILayout.MaxHeight(100)))
            {
                CheckLogListType(_logException);
            }
            if (GUILayout.Button("Clear",_guiSkin.button,GUILayout.MaxWidth(200), GUILayout.MaxHeight(100)))
            {
                ClearLog();
            }
            if (GUILayout.Button("+", _guiSkin.button, GUILayout.MaxWidth(200), GUILayout.MaxHeight(100)))
            {
                _fontSize++;
            }
            if (GUILayout.Button("-", _guiSkin.button, GUILayout.MaxWidth(200), GUILayout.MaxHeight(100)))
            {
                _fontSize--;
            }

            GUILayout.EndHorizontal();
            _scrollPositionText = GUILayout.BeginScrollView(_scrollPositionText, _guiSkin.horizontalScrollbar, _guiSkin.verticalScrollbar);
            foreach (var entry in _curLog)
            {
                var currentColor = GUI.contentColor;
                switch (entry.type)
                {
                    case LogType.Warning:
                        GUI.contentColor = Color.yellow;
                        break;
                    case LogType.Assert:
                        GUI.contentColor = Color.black;
                        break;
                    case LogType.Log:
                        GUI.contentColor = Color.green;
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        GUI.contentColor = Color.red;
                        break;
                    default:
                        break;
                }
                GUILayout.Label(entry.desc, _guiSkin.textArea);
                GUI.contentColor = currentColor;
            }
            GUILayout.EndScrollView();
        }

        private void OnGUI()
        {
#if UNITY_EDITOR
            return;
#endif
            if (!debugger) return;
            if (GUILayout.Button("Debugger",_guiSkin.button))
            {
                _isVisible = !_isVisible;
            }

            if (_isVisible)
            {
                _windowRect = GUILayout.Window(0, _windowRect, ConsoleWindow, "Debugger");

            }
        }

        private void OnDestroy()
        {
            if ((_fileWriter == null)) return;
            _fileWriter.Close();
            Application.logMessageReceivedThreaded -= LogCallback;
            Application.logMessageReceivedThreaded -= DebuggerCallBack;
        }




    }
}

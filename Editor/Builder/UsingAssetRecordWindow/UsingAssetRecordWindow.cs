using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace com.snake.framework
{
    namespace editor
    {
        public class Item : VisualElement
        {
            public string mAssetPath { get; private set; }
            public Item(string assetPath)
            {
                this.mAssetPath = assetPath;
                this.Add(new Label(assetPath));
            }
        }

        public class UsingAssetRecordWindow : EditorWindow
        {
            [MenuItem("SnakeTools/资源录制编辑器")]
            static public void ShowBuilderEditorWindow()
            {
                UsingAssetRecordWindow wnd = GetWindow<UsingAssetRecordWindow>();
                wnd.titleContent = new GUIContent("资源录制编辑器");
            }

            static public UsingAssetRecordWindow Instance { get; private set; }

            private ScrollView _scrollView;

            private bool _running = false;

            private Label _labState;
            private BuilderSetting _setting;
            private void OnEnable()
            {
                _setting = BuilderSetting.EditorGet();
                Instance = this;
            }

            private void OnDestroy()
            {
                Instance = null;
            }
            public void CreateGUI()
            {
                VisualElement root = rootVisualElement;
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(SnakeEditorUtility.GetPackagesPath() + "/Editor/Builder/UsingAssetRecordWindow/UsingAssetRecordWindow.uxml");
                VisualElement labelFromUXML = visualTree.Instantiate();
                _scrollView = labelFromUXML.Q<ScrollView>();

                labelFromUXML.Q<Button>("mBtnBegin").clicked += () => recordBegin();
                labelFromUXML.Q<Button>("mBtnEnd").clicked += () => recordEnd();
                labelFromUXML.Q<Button>("mBtnClear").clicked += () => clear();
                labelFromUXML.Q<Button>("mBtnSave").clicked += () => save();

                _labState = labelFromUXML.Q<Label>("mLabState");

                root.Add(labelFromUXML);
                refreshState();
            }

            private void recordBegin()
            {
                _running = true;
                refreshState();
            }

            private void recordEnd()
            {
                _running = false;
                refreshState();
            }

            private void refreshState()
            {
                _labState.text = "State" + (_running ? "录制中" : "空闲");
                _labState.style.color = _running ? Color.green : Color.red;
            }

            private void clear()
            {
                while (_scrollView.childCount > 0)
                    _scrollView.RemoveAt(0);
            }

            private void save()
            {
                string savePath = _setting.mUsingAssetsFilePath;
                if (System.IO.File.Exists(savePath))
                    System.IO.File.Delete(savePath);
                List<VisualElement> itemList = _scrollView.Children().ToList();
                using (TextWriter textWriter = File.CreateText(savePath))
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        Item item = itemList[i] as Item;
                        textWriter.WriteLine(item.mAssetPath);
                    }
                    textWriter.Flush();
                    textWriter.Close();
                }
            }

            public void AddUsingAsset(string assetPath)
            {
                if (_running == false)
                    return;
                _scrollView.Add(new Item(assetPath));
            }
        }
    }
}
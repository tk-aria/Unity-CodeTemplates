using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CodeTemplates
{


    [Serializable]
    public abstract class UnitySerializeWindow : ScriptableWizard
    {

        #region Event
        private delegate UnitySerializeWindow CreateWizard(Type klass, EditorConf editorConf);
        private delegate void Render(UnitySerializeWindow serializeWindow);

        private static Dictionary<int, CreateWizard> selectInstanceWindowKinds = new Dictionary<int, CreateWizard>
        {
            { 0, (Type klass, EditorConf editorConf)=>{ return DisplayWizard(editorConf.editorTitle, klass) as UnitySerializeWindow; }},
            { 1, (Type klass, EditorConf editorConf)=>{ return DisplayWizard(editorConf.editorTitle, klass, editorConf.footerButtons[0].name) as UnitySerializeWindow; }},
            { 2, (Type klass, EditorConf editorConf)=>{ return DisplayWizard(editorConf.editorTitle, klass, editorConf.footerButtons[0].name, editorConf.footerButtons[1].name) as UnitySerializeWindow; }},
        };

        private static Dictionary<RenderSetting.RenderType, Render> renderTypeSelector = new Dictionary<RenderSetting.RenderType, Render>
        { 
            { RenderSetting.RenderType.Default, (UnitySerializeWindow serializeWindow) => {
                    serializeWindow.RenderBaseGUI();
            }},
            { RenderSetting.RenderType.CustomOnly, (UnitySerializeWindow serializeWindow) => {
                    serializeWindow.OnRender();
            }},
            { RenderSetting.RenderType.Both, (UnitySerializeWindow serializeWindow) => {
                    serializeWindow.RenderBaseGUI();
                    serializeWindow.OnRender();
            }},
        };

        #endregion // Event End.

        #region Type

        [Serializable]
        public class RenderSetting
        {
            public enum RenderType
            {   
                Default,
                CustomOnly,
                Both
            }
            public RenderType renderType = RenderType.Both;
        }

        [Serializable]
        public class FooterButton
        {
            public enum Index
            {
                RightButtonIndex,
                LeftButtonIndex,
                ButtonMax,
            }

            public string name;
            public System.Action onClickHandler;
        }

        [Serializable][CreateAssetMenu(menuName = "ScriptableObject/EditorSupport/EditorConf")]
        public class EditorConf : ScriptableObject
        {

            public string editorTitle;

            public string editorCaption = "editor version @ ";
            public double editorVersion = 1.01;

            public RenderSetting renderSetting = new RenderSetting();

            public FooterButton[] footerButtons = new FooterButton[]{};

            public static EditorConf Create()
            {
                return ScriptableObject.CreateInstance<EditorConf>();
            }
        }

        [Serializable]
        public class Common
        {
            public EditorConf editorConf;
        }
        #endregion // Type End.

        #region Field

        [Header("Editor Settings")][HideInInspector]
        [SerializeField] protected EditorConf editorConf;

        #endregion // Field End.


        protected string editorTitle { get; set; }

        protected string editorCaption { get; set; }
        protected double editorVersion { get; set; }

        protected string leftButtonName { get; set; }
        protected string rightButtonName { get; set; }

        public abstract ScriptableWizard Initialize();
        public abstract void OnFinalize();
        public abstract void OnUpdate();
        public abstract void OnRender();
        public virtual void OnOpen(){}
        public virtual void OnClose(){}


        private void OnEnable()
        {
            OnOpen();
        }

        private void OnDisable()
        {
            OnClose();
        }

        private void OnDestory()
        {
            OnFinalize();
        }

        protected void OnWizardCreate()
        {
            if(editorConf == null) return;

            if (editorConf.footerButtons[(int)FooterButton.Index.RightButtonIndex] != null)
            {
                editorConf.footerButtons[(int)FooterButton.Index.RightButtonIndex].onClickHandler();
            }
        }

        protected void OnWizardOtherButton()
        {
            if (editorConf == null) return;

            if (editorConf.footerButtons[(int)FooterButton.Index.LeftButtonIndex] != null)
            {
                editorConf.footerButtons[(int)FooterButton.Index.LeftButtonIndex].onClickHandler();
            }
        }

        protected void OnWizardUpdate()
        {
            if (editorConf == null) return;

            helpString = editorConf.editorCaption + " v" + editorConf.editorVersion;
            OnUpdate();
        }

        protected override bool DrawWizardGUI()
        {
            RenderBaseGUI();
            OnRender();
            return true;
        }

        public bool RenderBaseGUI()
        {
            return base.DrawWizardGUI();
        }

        protected static UnitySerializeWindow ShowDisplay(Type klass, EditorConf editorConf)
        {
            int MAX = (int)FooterButton.Index.ButtonMax;
            var index = editorConf.footerButtons.Length > MAX ? MAX : editorConf.footerButtons.Length;
            var display = selectInstanceWindowKinds[index](klass, editorConf);
            display.editorConf = editorConf;
            return display;
        }

    }

}


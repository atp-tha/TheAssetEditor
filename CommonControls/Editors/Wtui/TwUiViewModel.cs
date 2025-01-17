﻿using CommonControls.Common;
using CommonControls.Editors.TextEditor;
using CommonControls.FileTypes.PackFiles.Models;
using CommonControls.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommonControls.Editors.Wtui
{
    public class TwUiViewModel : NotifyPropertyChangedImpl, IEditorViewModel, ITextEditorViewModel
    {
        private readonly PackFileService _pfs;
        PackFile _mainFile;
        List<UiTreeNode> _treNodeList = new List<UiTreeNode>();
        UiTreeNode _selectedNode;

        // Public attributes
        public ObservableCollection<UiTreeNode> TreeList { get; set; } = new ObservableCollection<UiTreeNode>();
        public UiTreeNode SelectedNode { get => _selectedNode; set { SetAndNotify(ref _selectedNode, value); NodeSelected(_selectedNode); } }

        public NotifyAttr<string> DisplayName { get; set; } = new NotifyAttr<string>();
        public PackFile MainFile { get => _mainFile; set { _mainFile = value; Load(_mainFile); } }
        public bool HasUnsavedChanges { get; set; }

        public ITextEditor TextEditor { get; private set; }
        bool _textChanged = false;
        string _text;
        public string Text
        {
            get => _text;
            set
            {
                SetAndNotify(ref _text, value);
                WriteTextToNode(value, SelectedNode);
                _textChanged = true;
            }
        }

        

        public TwUiViewModel(PackFileService pfs)
        {
            _pfs = pfs;
        }

        private void Load(PackFile mainFile)
        {
            DisplayName.Value = mainFile.Name;
            var bytes = mainFile.DataSource.ReadData();
            var xmlText = System.Text.Encoding.UTF8.GetString(bytes);
            var componentList = TwUiParser.LoadAllComponents(xmlText);
            var rootNote = TwUiParser.GenerateLayoutTree(xmlText, componentList);
            TreeList.Add(rootNote);
        }

        public void Close()
        {
        }

        public bool Save()
        {
            return true;
        }

        void NodeSelected(UiTreeNode selectedNode)
        {
            if (selectedNode == null)
                Text = "";
            else
                Text = selectedNode.XmlContent;
        }

        void WriteTextToNode(string text, UiTreeNode node)
        {
            if (node != null)
                node.XmlContent = text;
        }


        public void SetEditor(ITextEditor theEditor)
        {
            TextEditor = theEditor;
            TextEditor.ClearUndoStack();
        }
    }
}

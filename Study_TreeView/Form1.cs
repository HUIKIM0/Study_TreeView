using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Study_TreeView
{
    public partial class Form1 : Form
    {

        // Log Level 지정
        enum enLogLevel
        {
            Info,
            Warning,
            Error,
        }

        public Form1()
        {
            InitializeComponent();
        }


        //TreeNode 경로 
        private void btnTreeLoad_Click(object sender, EventArgs e)
        {
            TreeViewLoadPath(treeView1, tboxLocation.Text);
        }


        // 선택한 Node의 하위노드를 펼치기
        private void btnExtend_Click(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.ExpandAll();       //treeView1.ExpandAll();   다 펼치기
            }
        }

        //TreeNode 전체 닫기

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }


        #region TreeView Node Funtion Set

        //TreeView를 그리는 함수
        private void TreeViewLoadPath(TreeView treeView, string path)
        {

            treeView.Nodes.Clear();   //중복되서 찍히기 않게 기존의 TreeView 초기화

            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(path);  // DirectoryInfo class 선언
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));  // 다 합체된 완성된 폴더를 Add
        }


        // 경로에 따른 폴더 찾는 함수
        // 하위폴더 3번째 -> 하위폴더 2번째 -> 하위폴더 1번째 -> Root
        // 아래에서 부터 위로 합체되서 올라간다는 느낌(재귀함수)
        private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            TreeNode directoryNode = new TreeNode(directoryInfo.Name);


            //경로에 포함되는 폴더 찾기 foreach
            foreach (var directory in directoryInfo.GetDirectories())   // 해당 경로의 디렉토리를 배열로 가져옴
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));   // 경로를 재귀 함수로 계속 호출 하면서 하위 노드 들을 찾아 옴


            // File 을 가지고 오기 위한 Node
            //foreach (var file in directoryInfo.GetFiles())
            //    directoryNode.Nodes.Add(new TreeNode(file.Name));


            return directoryNode;
        }
        #endregion


        #region Log OverLoading


        #endregion

    }
}

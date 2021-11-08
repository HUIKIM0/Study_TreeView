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

        /// <summary>
        /// 폴더 이해하기^^........
        /// 
        ///     ex) D:\dev\C#Work    를 tboxLocatio에 입력함
        ///     TreeView에서 C#Work안의 Study_TreeView 폴더  더블클릭
        ///     lboxCommand에 C#Work\Study_TreeView 가 Add됨(한 단계 상위+선택Node)
        ///     lboxCommand 클릭 시 Study_TreeView안의 폴더+파일 내용 보여줌(선택Node에 담긴)
        ///     
        ///     
        /// 1)
        ///     treeView1.SelectedNode.FullPath;   ->  C#Work\Study_TreeView
        ///     한단계 상위 폴더\선택 폴더 까지만 된다(D:\dev 까진 안 찍힘) Root는 X
        ///2)
        ///      Path.GetDirectoryName(path);    ->D:\dev
        ///      예시에서의 마지막 폴더인 C#Work는 잘림. 앞의 경로 D:\dev만
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 



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


        // 경로 btn  #region TreeView Node Funtion Set 참조
        private void btnTreeLoad_Click(object sender, EventArgs e)
        {
            TreeViewLoadPath(treeView1, tboxLocation.Text);
        }


        // 선택한 Node의 하위노드를 펼치기  Expand btn
        private void btnExtend_Click(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.ExpandAll();       //treeView1.ExpandAll();   다 펼치기
            }
        }

        //TreeNode 전체 닫기    Collapse btn
        private void btnCollapse_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }



        // TreeView 더블클릭 -> lboxCommand에 Node 경로를 넣는다
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            string strSelectPath = treeView1.SelectedNode.FullPath;   //한단계 상위Node ~ 선택한Node까지 FullPath

            if (lboxCommand.Items.Contains(strSelectPath))
            {
                Log(enLogLevel.Warning, "선택한 폴더는 이미 Command에 등록되어 있습니다.");
                //MessageBox.Show("선택한 폴더는 이미 Command에 등록되어 있습니다");
                return;
            }

            else
            {
                lboxCommand.Items.Add(strSelectPath);
                //lboxCommand.Items.Add(treeView1.SelectedNode.FullPath);
            }
        }


        // lboxCommand 클릭 -> 아래의 TextBox에 폴더와 파일 보여줌
        private void lboxCommand_MouseClick(object sender, MouseEventArgs e)
        {

            if (lboxCommand.SelectedItem == null) return;   // 에러방지


            StringBuilder sb = new StringBuilder();
            string dirpath = SearchPath();

            if (string.IsNullOrEmpty(dirpath))
            {
                Log(enLogLevel.Warning, "경로가 입력되지 않았습니다.");
                return;
            }


            if (Directory.Exists(dirpath))
            {
                DirectoryInfo dInfo = new DirectoryInfo(dirpath);   // DirectoryInfo class 선언

                //디렉토리 찾기(node에 넣어줄 필요X 찾아서 TextBox에 넣어주면 됨
                foreach (var directory in dInfo.GetDirectories())
                {
                    sb.Append($"[Folder] {directory} \r\n");
                }

                //파일 찾기
                foreach (var file in dInfo.GetFiles())
                {
                    sb.Append($"  [File] {file} \r\n");
                }

                tboxFile.Text = sb.ToString();
            }
        }


        // lboxCommand 더블클릭 -> 선택된 Item 삭제
        private void lboxCommand_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            lboxCommand.Items.RemoveAt(lboxCommand.SelectedIndex);

        }

        

        // FileSystem import WHY 안되는가^^........ Copy btn
        private void btnCopy_Click(object sender, EventArgs e)
        {
            string Path = SearchPath();  
            // D\temp\20210101_18:23:00
            string BackUpPath = $@"{tboxBackUp.Text}\{DateTime.Now:yyyyMMdd_hh:mm:ss}";

            Log(enLogLevel.Info,$"원본 경로 {Path}");
            Log(enLogLevel.Info, $"백업 경로 {BackUpPath}");

            //FileSystem.CopyDirectory(Path, BackUpPath, UIOption.AllDialogs);

            Log(enLogLevel.Info, "경로 BackUp을 완료 했습니다");
        }



        #region TreeView Node Funtion Set
        //TreeView를 그리는 함수
        private void TreeViewLoadPath(TreeView treeView, string path)
        {
            if (string.IsNullOrEmpty(tboxLocation.Text))
            {
                Log(enLogLevel.Warning, "경로가 입력되지 않았습니다.");
            }

            treeView.Nodes.Clear();   // 클릭 할 때 마다 중복해서 찍히기 않게 기존의 TreeView 초기화

            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(path);  // DirectoryInfo class 선언 ★
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));  // 다 합체된 완성된 폴더를 Add ★
        }


        // 경로에 따른 폴더(노드) 찾는 함수
        // 하위폴더 3번째 -> 하위폴더 2번째 -> 하위폴더 1번째 -> Root
        // 아래에서 부터 위로 합체되서 올라간다는 느낌(재귀함수)
        private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            TreeNode directoryNode = new TreeNode(directoryInfo.Name);


            //디렉토리 Node 찾기 foreach
            foreach (var directory in directoryInfo.GetDirectories())   // 해당 경로의 디렉토리를 배열로 가져옴
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));   // 경로를 재귀 함수로 계속 호출 하면서 하위 노드 들을 찾아 옴


            // 파일 Node 찾기 foreach
            //foreach (var file in directoryInfo.GetFiles())
            //    directoryNode.Nodes.Add(new TreeNode(file.Name));


            return directoryNode;
        }
        #endregion



        #region  SearchPath /Log OverLoading 
        private void Log(enLogLevel eLevel, string strLog)
        {
            DateTime dt = DateTime.Now;
            string LogInfo = $"{dt:yyyy-mm-dd hh:mm:ss:fff} [{eLevel.ToString()}] {strLog}";
            lboxLog.Items.Insert(0, LogInfo);
        }


        //★ 경로 완성시키기!!!!!!
        //Root 경로 ~ lboxCommand에서 선택한 경로 까지 합쳐줌
        private string SearchPath()
        { 
            string path = tboxLocation.Text;    // D:\dev\C#Work

            var lastFolder = Path.GetDirectoryName(path);       // D:\dev  !마지막폴더인 C#Work는 짤림!
            string strpath = lboxCommand.SelectedItem.ToString();     // C#Work\Study_TreeView 

            string dirPath = $@"{lastFolder}\{strpath}";  // {D:\dev}\{C#Work\Study_TreeView}

            return dirPath;
        }
        #endregion


    }
}

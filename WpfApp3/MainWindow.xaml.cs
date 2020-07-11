using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    struct Coordinate
    {
        public int row;
        public int col;
    }


    public partial class MainWindow : Window
    {
        ArrayList myColorSet;
        Random rnd;
        int rowCount, columnCount;
        Border[,] myBorders;
        Label labelSkor;
        DockPanel dock;
        static int currentBlockRow1;
        static int currentBlockColumn;
        static int currentBlockColumnCount=0;
        static int myScore=0;
        DispatcherTimer timer;
        SortedSet<Coordinates> coordinatesList;


        public MainWindow()
        {
            InitializeComponent();

            init();
            
        }

        public void init()
        {
            coordinatesList = new SortedSet<Coordinates>();
            myColorSet = new ArrayList();
            rnd = new Random();
            
            DefineColors();
         

            rowCount = mGrid.RowDefinitions.Count;
            columnCount = mGrid.ColumnDefinitions.Count;

            myBorders = new Border[rowCount, columnCount];

            dock = new DockPanel();
            dock.Background = Brushes.AliceBlue;
            dock.SetValue(Grid.RowProperty, 0);
            dock.SetValue(Grid.ColumnProperty, 0);
            dock.SetValue(Grid.ColumnSpanProperty, 5);
            mGrid.Children.Add(dock);

            CreateScoreBar();
            CreateBorders();
            

        }

        public void BlockGenerate()
        {
            int generetadColumn = rnd.Next(0, columnCount);
            int initialRow = 1;

            myBorders[initialRow, generetadColumn].Background = (SolidColorBrush)myColorSet[rnd.Next(0, myColorSet.Count)]; ;
            myBorders[initialRow + 1, generetadColumn].Background = (SolidColorBrush)myColorSet[rnd.Next(0, myColorSet.Count)]; ;
            myBorders[initialRow + 2,generetadColumn].Background = (SolidColorBrush)myColorSet[rnd.Next(0, myColorSet.Count)]; ;


            currentBlockRow1 = initialRow;
            currentBlockColumn = generetadColumn;
            
            MoveBlock();
        }
        
        private void MoveBlock()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.3);
            timer.Tick += new EventHandler(MovingBlock);
            timer.Start();
        }

        private void MovingBlock(object sender, EventArgs e)
        {
            if (currentBlockRow1+3<rowCount)
            {

                //currentBlockRow1 + 3 == rowCount
             

                if (myBorders[(currentBlockRow1 + 3), currentBlockColumn].Background == Brushes.White)               
                {
                    // Beyaz gördügünde

                    myBorders[currentBlockRow1 + 3, currentBlockColumn].Background = myBorders[currentBlockRow1 + 2, currentBlockColumn].Background;
                    myBorders[currentBlockRow1 + 2, currentBlockColumn].Background = myBorders[currentBlockRow1 + 1, currentBlockColumn].Background;
                    myBorders[currentBlockRow1 + 1, currentBlockColumn].Background = myBorders[currentBlockRow1, currentBlockColumn].Background;
                    myBorders[currentBlockRow1, currentBlockColumn].Background = Brushes.White;
                    currentBlockRow1++;

                }
                else 
                {
                    //Baska bir renge denk geldiginde kontrol yapacağız
                    if (currentBlockRow1 == 1)
                    {
                        MessageBox.Show("Game Over !" + Environment.NewLine +"Your score is :" +labelSkor.Content);
                        timer.Stop();
                    }
                    else
                    {
                        timer.Stop();
                        Control();
                        
                        //BlockGenerate();
                    }

                }
            }
            else if(currentBlockRow1+3 == rowCount)
            {
                timer.Stop();
                Control();
                
                //BlockGenerate();

            }
            else
            {
                timer.Stop();
                MessageBox.Show("Game Over");
            }
        }

        private void Control()
        {         

            //Row Kontrol
            #region

            RowControl(currentBlockRow1);
            RowControl(currentBlockRow1 + 1);
            RowControl(currentBlockRow1 + 2);

            //Cross Kontrol
            #endregion

            CrossControl(currentBlockRow1);
            CrossControl(currentBlockRow1 + 1);
            CrossControl(currentBlockRow1 + 2);


            // Column Control
            #region
            int repeatedColorCount = 1;
            int repeatedColorEndRow = 0;
            SolidColorBrush repeatedColor = null;

            for (int i = 1; i < rowCount - 1; i++)
            {
                if (myBorders[i, currentBlockColumn].Background == myBorders[i + 1, currentBlockColumn].Background && ((SolidColorBrush)myBorders[i, currentBlockColumn].Background).Color != Colors.White)
                {
                    repeatedColorEndRow = i + 1;
                    repeatedColorCount++;
                    if (repeatedColorCount >= 3)
                    {
                        repeatedColor = myBorders[i + 1, currentBlockColumn].Background as SolidColorBrush;
                    }
                }
                else
                {
                    if (repeatedColorCount < 3)
                    {
                        repeatedColorCount = 1;
                    }
                }
            }
            if (repeatedColorCount >= 3)
            {
                int repeatedColorStart = repeatedColorEndRow - repeatedColorCount;
                for (int kj = repeatedColorStart + 1; kj < repeatedColorEndRow + 1; kj++)
                {
                    //myBorders[kj, currentBlockColumn].Background = Brushes.White;
                    coordinatesList.Add(new Coordinates
                    {
                        row = kj,
                        col = currentBlockColumn
                    });
                }

                //if (myBorders[repeatedColorStart, currentBlockColumn].Background != Brushes.White)
                //{
                //    for (int ij = repeatedColorStart; ij > 0; ij--)
                //    {
                //        int lastWhiteCol = 0;
                //        for (int ik = repeatedColorStart; ik < rowCount; ik++)
                //        {

                //            if (myBorders[ik, currentBlockColumn].Background == Brushes.White)
                //            {
                //                lastWhiteCol = ik;
                //            }
                //        }
                //        myBorders[lastWhiteCol, currentBlockColumn].Background = myBorders[ij, currentBlockColumn].Background;
                //        myBorders[ij, currentBlockColumn].Background = Brushes.White;
                //    }
                //}
                var skor = Convert.ToInt32(labelSkor.Content);
                var newSkor = skor + repeatedColorCount;

                labelSkor.Content = newSkor.ToString();
            }

            #endregion

            RemoveColors();

        }
        private void RemoveColors()
        {
            String st="";
            foreach(var item in coordinatesList)
            {
                st += item.row.ToString() + " ";
                myBorders[item.row, item.col].Background = Brushes.White;
                DoubleAnimation da = new DoubleAnimation();
                da.From = 30;
                da.To = 100;
                da.Duration = new Duration(TimeSpan.FromSeconds(1));
                myBorders[item.row, item.col].BeginAnimation(Button.HeightProperty, da);
                ShiftBlock(item.row, item.col);                
            }
            //await Task.Delay(300);
            coordinatesList.Clear();
            BlockGenerate();
        }

        private void ChangeScoreboard(int inc)
        {
            var val = Convert.ToInt32(labelSkor.Content) ;
            val += inc;
            labelSkor.Content = val.ToString();
        }

        private void CrossControl(int row)
        {
            int lastLeftRow = row;
            int lastLeftCol = currentBlockColumn;

            while (lastLeftRow < rowCount - 1 && lastLeftCol > 0)
            {
                lastLeftRow++;
                lastLeftCol--;
            }
            int lastLeftRowCopy = lastLeftRow;
            int lastLeftColCopy = lastLeftCol;
            //Sag son kutu

            int lastRightRow = row;
            int lastRightCol = currentBlockColumn;

            while (lastRightCol < 4 && lastRightRow < rowCount - 1)
            {
                lastRightCol++;
                lastRightRow++;
            }
            int lastRightRowCopy = lastRightRow;
            int lastRightColCopy = lastRightCol;

            // sol son kutu ve sag son kutudan capraz sekilde yukarı cıkıp tekrar eden renkleri silelim
            //sol icin


            #region

            int repCol = 1;
            SolidColorBrush removeCol1 = null;
            Coordinate leftLastCoordinate = new Coordinate();

            while (lastLeftRow > 1 && lastLeftCol < 4)
            {

                if (myBorders[lastLeftRow, lastLeftCol].Background == myBorders[lastLeftRow - 1, lastLeftCol + 1].Background && ((SolidColorBrush)myBorders[lastLeftRow, lastLeftCol].Background).Color != Colors.White)
                {
                    repCol++;
                    if (repCol > 2)
                    {
                        removeCol1 = myBorders[lastLeftRow, lastLeftCol].Background as SolidColorBrush;
                        leftLastCoordinate.row = lastLeftRow - 1;
                        leftLastCoordinate.col = lastLeftCol+1;
                    }
                }

                // myBorders[lastLeftRow, lastLeftCol].Background = Brushes.Black;
                lastLeftRow--;
                lastLeftCol++;

            }

            if(removeCol1 != null && repCol>2)
            {
                int mRow = leftLastCoordinate.row;
                int mCol = leftLastCoordinate.col;
                while (mRow > 1 && mCol <= 4  && repCol>0)
                {
                    //if (myBorders[mRow, mCol] != myBorders[row, currentBlockColumn])
                    //{
                    //myBorders[mRow, mCol].Background = Brushes.White;
                    coordinatesList.Add(new Coordinates
                    {
                        row = mRow,
                        col = mCol
                    });
                        ChangeScoreboard(1);
                        //ShiftBlock(mRow, mCol);
                    //}
                    mRow++;
                    mCol--;
                    repCol--;
                }
            }
            //eski kod
            #region
            //if (removeCol1 != null)
            //{
            //    while (lastLeftRowCopy > 1 && lastLeftColCopy < 4)
            //    {
            //        if (myBorders[lastLeftRowCopy, lastLeftColCopy].Background == removeCol1 && 
            //            myBorders[lastLeftRowCopy,lastLeftColCopy]!=myBorders[row,currentBlockColumn])
            //        {
            //            myBorders[lastLeftRowCopy, lastLeftColCopy].Background = Brushes.White;
            //            ChangeScoreboard(1);
            //        }
            //        lastLeftRowCopy--;
            //        lastLeftColCopy++;
            //    }
            //}
            #endregion

            #endregion


            //sag icin
            #region 
            int repCol2 = 1;
            SolidColorBrush removeCol2 = null;
            Coordinate rightLastCoordinate = new Coordinate();

            while (lastRightRow > 1 && lastRightCol > 0)
            {

                if (myBorders[lastRightRow, lastRightCol].Background == myBorders[lastRightRow - 1, lastRightCol - 1].Background && ((SolidColorBrush)myBorders[lastRightRow, lastRightCol].Background).Color != Colors.White)
                {
                    repCol2++;
                    if (repCol2 > 2)
                    {
                        removeCol2 = myBorders[lastRightRow, lastRightCol].Background as SolidColorBrush;
                        rightLastCoordinate.row = lastRightRow - 1;
                        rightLastCoordinate.col = lastRightCol - 1;
                    }
                }
                lastRightRow--;
                lastRightCol--;
            }

           

            if (removeCol2 != null && repCol2>2)
            {

                int mmRow = rightLastCoordinate.row;
                int mmCol = rightLastCoordinate.col;

                while (mmRow < 11 && mmCol <5)
                {
                    //if (myBorders[mmRow, mmCol] != myBorders[row, currentBlockColumn])
                    //{
                    //myBorders[mmRow, mmCol].Background = Brushes.White;
                    coordinatesList.Add(new Coordinates
                    {
                        row = mmRow,
                        col = mmCol
                    });
                    ChangeScoreboard(1);
                        //ShiftBlock(mmRow, mmCol);
                    //}
                    mmRow++;
                    mmCol++;
                }
            }
            
            #endregion
        }

        private void ShiftBlock(int row , int col)
        {
            for (int br = row - 1; br > 0; br--)
            {
                if (myBorders[br, col].Background != Brushes.White)
                {
                    myBorders[br + 1, col].Background = myBorders[br, col].Background;
                }
                else
                {
                    myBorders[br + 1, col].Background = Brushes.White;
                    break;
                }
            }
        }

        private void RowControl(int row)
        {
            int repColColorCount = 1;
            int repeatedColorEndCol = 0;
            SolidColorBrush repeatedColorCol = null;

            for (int j = 0; j < columnCount-1; j++)
            {
                if (myBorders[row, j].Background == myBorders[row, j + 1].Background && myBorders[row, j + 1].Background != Brushes.White)
                {
                    repColColorCount++;
                    repeatedColorEndCol = j + 1;
                    if (repColColorCount >= 3)
                    {
                        repeatedColorCol = myBorders[row, j].Background as SolidColorBrush;
                    }
                }
                else
                {
                    if (repColColorCount < 3)
                    {
                        repColColorCount = 1;
                    }
      
            }
            }

            if (repColColorCount>=3)
            {

               int repeatedColorStartCol = repeatedColorEndCol - repColColorCount;
               for (repeatedColorStartCol += 1; repeatedColorStartCol < repeatedColorEndCol + 1; repeatedColorStartCol++)
               {
                    //blogumuzun kolonunda bulunan rengi beyaza boyamasin cünkü bunu column control kısmında kullanacagiz
                    //if (repeatedColorStartCol == currentBlockColumn)
                    //{
                    //    continue;
                    //}
                    //myBorders[row, repeatedColorStartCol].Background = Brushes.White;
                    ChangeScoreboard(1);
                    coordinatesList.Add(new Coordinates
                    {
                        row = row,
                        col = repeatedColorStartCol
                    });

                    //for (int br = row-1; br>0; br--)
                    //{
                    //    if (myBorders[br, repeatedColorStartCol].Background != Brushes.White)
                    //    {
                    //        myBorders[br + 1, repeatedColorStartCol].Background = myBorders[br, repeatedColorStartCol].Background;
                    //    }
                    //    else
                    //    {
                    //        myBorders[br+1, repeatedColorStartCol].Background = Brushes.White;
                    //        break;
                    //    }
                    //}
               }
                
            }
            


        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            DockPanel parent = (DockPanel)btn.Parent;
            parent.Children.Clear();

            Label label1 = new Label();
            label1.VerticalAlignment = VerticalAlignment.Center;
            label1.HorizontalAlignment = HorizontalAlignment.Stretch;
            label1.Content = "Score : ";
            labelSkor = new Label();
            labelSkor.VerticalAlignment = VerticalAlignment.Center;
            labelSkor.HorizontalAlignment = HorizontalAlignment.Stretch;
            labelSkor.Content = "0";
            Button btnExit = new Button();
            btnExit.Content = "Reset !";
            btnExit.Width = 50;
            btnExit.HorizontalAlignment = HorizontalAlignment.Right;
            btnExit.VerticalAlignment = VerticalAlignment.Top;
            btnExit.Click += ResetClick;

            parent.Children.Add(label1);
            parent.Children.Add(labelSkor);
            parent.Children.Add(btnExit);

            BlockGenerate();
            
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            dock.Children.Clear();
            CreateScoreBar();
            RemoveColorFromBorders();
           
        }

        private void RemoveColorFromBorders()
        {
            for (int i = 1;  i < rowCount ; i++){
                for(int j = 0; j<columnCount; j++)
                {
                    myBorders[i, j].Background = Brushes.White;
                }
            }
        }

        private void CreateBorders()
        {
            for (int i = 0; i < rowCount; i++)
            {

                for (int j = 0; j < columnCount; j++)
                {
                    Border mBorder = new Border();                   
                    mBorder.BorderThickness = new Thickness(1, 1, 1, 1);
                    mBorder.SetValue(Grid.RowProperty, i);
                    mBorder.SetValue(Grid.ColumnProperty, j);
                    
                    if (i > 1)
                    {
                        mBorder.Background = Brushes.White;
                    }
                    mBorder.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(border1_MouseLeftButtonDown);
                    myBorders[i, j] = mBorder;

                    mGrid.Children.Add(mBorder);
                }
            }

            
        }

        private void border1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border mBorder = (Border)sender;
            int valColumn=Convert.ToInt32(mBorder.GetValue(Grid.ColumnProperty));
            int valRow = Convert.ToInt32(mBorder.GetValue(Grid.RowProperty));
            if (valColumn>currentBlockColumn && myBorders[currentBlockRow1+2,currentBlockColumn+1].Background == Brushes.White)
            {
                myBorders[currentBlockRow1, currentBlockColumn + 1].Background = myBorders[currentBlockRow1, currentBlockColumn].Background;
                myBorders[currentBlockRow1+1, currentBlockColumn + 1].Background = myBorders[currentBlockRow1+1, currentBlockColumn].Background;
                myBorders[currentBlockRow1+2, currentBlockColumn + 1].Background = myBorders[currentBlockRow1+2, currentBlockColumn].Background;

                myBorders[currentBlockRow1, currentBlockColumn].Background = Brushes.White;
                myBorders[currentBlockRow1 + 1, currentBlockColumn].Background = Brushes.White;
                myBorders[currentBlockRow1 + 2, currentBlockColumn].Background = Brushes.White;
                currentBlockColumn += 1;
            }
            else if (valColumn < currentBlockColumn && myBorders[currentBlockRow1 + 2, currentBlockColumn - 1].Background == Brushes.White)
            {
                myBorders[currentBlockRow1, currentBlockColumn - 1].Background = myBorders[currentBlockRow1, currentBlockColumn].Background;
                myBorders[currentBlockRow1 + 1, currentBlockColumn - 1].Background = myBorders[currentBlockRow1 + 1, currentBlockColumn].Background;
                myBorders[currentBlockRow1 + 2, currentBlockColumn - 1].Background = myBorders[currentBlockRow1 + 2, currentBlockColumn].Background;

                myBorders[currentBlockRow1, currentBlockColumn].Background = Brushes.White;
                myBorders[currentBlockRow1 + 1, currentBlockColumn].Background = Brushes.White;
                myBorders[currentBlockRow1 + 2, currentBlockColumn].Background = Brushes.White;

                currentBlockColumn -= 1;
            }
            

            if(valRow==currentBlockRow1 || valRow==(currentBlockRow1+1) || valRow == (currentBlockRow1 + 2))
            {
                SolidColorBrush lastColor = myBorders[currentBlockRow1 + 2, currentBlockColumn].Background as SolidColorBrush;
                myBorders[currentBlockRow1 + 2, currentBlockColumn].Background = myBorders[currentBlockRow1 + 1, currentBlockColumn].Background;
                myBorders[currentBlockRow1 + 1, currentBlockColumn].Background = myBorders[currentBlockRow1, currentBlockColumn].Background;
                myBorders[currentBlockRow1, currentBlockColumn].Background = lastColor;
            }

        }

        public void DefineColors()
        {
            myColorSet.Add(Brushes.Red);
            myColorSet.Add(Brushes.Blue);
            myColorSet.Add(Brushes.Yellow);
            myColorSet.Add(Brushes.Green);
            myColorSet.Add(Brushes.Orange);
        }

        private void CreateScoreBar()
        {
            Button btn = new Button();
            btn.Click += StartClick;
            btn.Content = "Start Game";
            dock.Children.Add(btn);
        }


        public class Coordinates : IComparable //: IEquatable<Coordinates>
        {
            public int row;
            public int col;

            public int CompareTo(object obj)
            {
                var coor = (Coordinates)obj;
                if (row < coor.row)
                {
                    return -1;
                }
                if(row>coor.row)
                {
                    return 1;
                }
                if (row == coor.row )
                {
                    return 1;
                }
                return 0;
            }

            //public bool Equals(Coordinates other)
            //{
            //    if(row == other.row && col == other.col)
            //    {
            //        return true;
            //    }
            //    return false;
            //}

            //public override int GetHashCode()
            //{
            //    return row.GetHashCode();
            //}


        }

        public class BandNameComparer : IComparer<Coordinates>
        {
            public int Compare(Coordinates x, Coordinates y)
            {
                return x.row.CompareTo(y.row);
            }
        }


    }
}

using Btns;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Bank> banks;
        List<Move> moves;
        StackPanel movesPanelTmp;
        public MainWindow()
        {
            InitializeComponent();
            banks = new List<Bank>();
            moves = new List<Move>();
        }

        /*     MENU     */
        //Home click
        private void AnimButton_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            homeGrid.IsEnabled = true;
            homeGrid.Visibility = Visibility.Visible;
            banksGrid.IsEnabled = false;
            banksGrid.Visibility = Visibility.Hidden;
            movesGrid.IsEnabled = false;
            movesGrid.Visibility = Visibility.Hidden;
        }
        //Banks click
        private void AnimButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            homeGrid.IsEnabled = false;
            homeGrid.Visibility = Visibility.Hidden;
            banksGrid.IsEnabled = true;
            banksGrid.Visibility = Visibility.Visible;
            movesGrid.IsEnabled = false;
            movesGrid.Visibility = Visibility.Hidden;
        }
        //Moves click
        private void AnimButton_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            homeGrid.IsEnabled = false;
            homeGrid.Visibility = Visibility.Hidden;
            banksGrid.IsEnabled = false;
            banksGrid.Visibility = Visibility.Hidden;
            movesGrid.IsEnabled = true;
            movesGrid.Visibility = Visibility.Visible;

            moveIn.Items.Clear();
            moveOut.Items.Clear();
            foreach(var b in banks)
            {
                moveIn.Items.Add(b.Text);
                moveOut.Items.Add(b.Text);
            }
        }
        /********************/

        /*     BANKS       */
        //Add new bank click
        private void AnimButton_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (bankNameBox.Text.Length > 0)
            {
                banks.Add(new Bank());
                banks[banks.Count - 1].DeletePress += MainWindow_DeletePress;
                banks[banks.Count - 1].NameChange += BankNameChange;
                banks[banks.Count - 1].TextInputEnable = false;
                banks[banks.Count - 1].Text = bankNameBox.Text;
                bankNameBox.Text = "";
                banksStack.Children.Add(banks[banks.Count - 1]);
                banksCount.Content = "כמות" + ":" + banks.Count;
            }
            else MessageBox.Show("אין אפשרות להוסיף בנק חדש בלי שם");
        }

        //Delete bank click
        private void MainWindow_DeletePress(object sender)
        {
            var item = banks.Find(i => sender.ToString() == i.Text);
            banksStack.Children.Remove(item);
            banks.Remove(item);
            banksCount.Content = "כמות" + ":" + banks.Count;
        }
        private void BankNameChange(object sender)
        {
            Bank b = banks.Find(i => i.Text == sender.ToString());
            foreach (var i in moves)
            {
                int index = banks.IndexOf(b);
                if (index == i.Out.SelectedIndex)
                {
                    i.Out.Items[index] = b.Text;
                    i.Out.SelectedIndex = index;
                    goto SKIP;
                }
                if (index == i.In.SelectedIndex)
                {
                    i.In.Items[index] = b.Text;
                    i.In.SelectedIndex = index;
                    goto SKIP;
                }
                i.In.Items[index] = b.Text;
                i.Out.Items[index] = b.Text;
            SKIP:;
            }
        }
        /***************/

        /*     MOVES    */
        //Create move click
        private void CreateMove(object sender, MouseButtonEventArgs e)
        {
            if(moveOut.SelectedIndex != -1 &&moveIn.SelectedIndex != -1 &&
                moveSum.Text.Length>0 && moveDate.SelectedDate != null && moveName.Text.Length>0)
            {
                moves.Add(new Move());
                moves[moves.Count - 1].Date = moveDate.SelectedDate;
                moves[moves.Count - 1].Name = moveName.Text;
                foreach(var v in moveOut.Items)
                {
                    moves[moves.Count - 1].Out.Items.Add(v);
                    moves[moves.Count - 1].In.Items.Add(v);
                }
                moves[moves.Count - 1].Out.SelectedIndex = moveOut.SelectedIndex;
                moves[moves.Count - 1].In.SelectedIndex = moveIn.SelectedIndex;
                moves[moves.Count - 1].Price = int.Parse(moveSum.Text);
                moves[moves.Count - 1].Var = moveVar.IsChecked;
                moves[moves.Count - 1].BankInId = banks[moveIn.SelectedIndex].Id;
                moves[moves.Count - 1].MoveChange += MoveChange;
                banks[moveIn.SelectedIndex].OutAmount = (int.Parse(banks[moveIn.SelectedIndex].OutAmount) + 1).ToString();
                banks[moveIn.SelectedIndex].Sum = (int.Parse(banks[moveIn.SelectedIndex].Sum) + int.Parse(moveSum.Text)).ToString();
                banks[moveOut.SelectedIndex].InAmount = (int.Parse(banks[moveOut.SelectedIndex].InAmount) + 1).ToString();
                banks[moveOut.SelectedIndex].Sum = (int.Parse(banks[moveOut.SelectedIndex].Sum) - int.Parse(moveSum.Text)).ToString();
                moves[moves.Count - 1].BankOutId = banks[moveOut.SelectedIndex].Id;
                moves[moves.Count - 1].DeletePress += DeletePress;
                movesStack.Children.Add(moves[moves.Count - 1]);
                UpdateMovesCount();
            }
            else MessageBox.Show("חובה למלא את כל השדות");
        }
        //Search move click
        private void SearchMove(object sender, MouseButtonEventArgs e)
        {
            movesStack.Children.RemoveRange(1, movesStack.Children.Count - 1);
            foreach (var i in moves)
            {
                movesStack.Children.Add(i);
            }
            movesPanelTmp = new StackPanel();
            for (int i = 0; i<movesStack.Children.Count;i++)
            {
                if(movesStack.Children[i] is Move)
                {
                    Move c = (Move)movesStack.Children[i];
                    if (
                    (moveOut.SelectedIndex != -1 && banks.FindIndex(k=>k.Id == c.BankOutId) != moveOut.SelectedIndex)||
                    (moveIn.SelectedIndex != -1 && banks.FindIndex(k => k.Id == c.BankInId) != moveIn.SelectedIndex)||
                    (moveSum.Text.Length > 0 && int.Parse(moveSum.Text) != c.Price)||
                    (moveDate.SelectedDate!=null && moveDate.SelectedDate != c.Date)||
                    (moveName.Text.Length > 0 && moveName.Text != c.Name)||
                    (moveVar.IsChecked != c.Var)
                    )
                    {
                        movesStack.Children.Remove(c);
                        i--;
                    }
                }
            }

        }
        private void DeletePress(object sender)
        {
            Move m = moves.Find(i => i.Name == sender.ToString());
            Bank inBank = banks.Find(i => i.Id == m.BankInId);
            Bank outBank = banks.Find(i => i.Id == m.BankOutId);
            inBank.Sum = (int.Parse(inBank.Sum) - m.Price).ToString();
            inBank.OutAmount = (int.Parse(inBank.OutAmount) - 1).ToString();
            outBank.Sum = (int.Parse(outBank.Sum) + m.Price).ToString();
            outBank.InAmount = (int.Parse(outBank.InAmount) - 1).ToString();
            movesStack.Children.Remove(m);
            moves.Remove(m);
            UpdateMovesCount();
        }
        private void UpdateMovesCount()
        {
            moveOut.SelectedIndex = -1;
            moveIn.SelectedIndex = -1;
            moveSum.Text = "";
            moveDate.SelectedDate = null;
            moveName.Text = "";
            int regularCount = moves.Where(i => i.Var == false).Count();
            moveCountBasic.Content = "רגילות" + ":" + regularCount;
            moveCountVar.Content = "משתנות" + ":" + (moves.Count - regularCount);
            moveCountTotal.Content = "כולל" + ":" + moves.Count;
        }
        //Clean filter
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            movesStack.Children.RemoveRange(1, movesStack.Children.Count - 1);
            foreach(var i in moves)
            {
                movesStack.Children.Add(i);
            }
            UpdateMovesCount();
        }
        private void MoveChange(object sender)
        {
            foreach(Bank b in banks)
            {
                b.InAmount = "0";
                b.OutAmount = "0";
                b.Sum = "0";
            }
            foreach(Move m in moves)
            {
                m.BankInId = banks[m.In.SelectedIndex].Id;
                m.BankOutId = banks[m.Out.SelectedIndex].Id;
                Bank bIn = banks.Find(i => i.Id == m.BankInId);
                Bank bOut = banks.Find(i => i.Id == m.BankOutId);
                bIn.OutAmount = (int.Parse(bIn.OutAmount) + 1).ToString();
                bIn.Sum = (int.Parse(bIn.Sum) + m.Price).ToString();
                bOut.InAmount = (int.Parse(bOut.InAmount) + 1).ToString();
                bOut.Sum = (int.Parse(bOut.Sum) - m.Price).ToString();
            }
            UpdateMovesCount();
        }
        /*     HELPERS    */
        //Validate only numeric value on text box
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        /******************/
    }
}

using System.Collections.Generic;

namespace swengine.desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public List<int> DesignList
    {
        get
        {
            List<int> temp = new();
            for (int i = 0; i <= 15; i++)
            {
                temp.Add(i);
            }

            return temp;
        }
    }
}
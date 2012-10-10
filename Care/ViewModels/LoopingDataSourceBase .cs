using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using System.Collections.Generic;

namespace Care
{
    // abstract the reusable code in a base class 
    // this will allow us to concentrate on the specifics when implementing deriving looping data source classes
    public abstract class LoopingDataSourceBase : ILoopingSelectorDataSource
    {
        private object selectedItem;
        #region ILoopingSelectorDataSource Members
        public abstract object GetNext(object relativeTo);
        public abstract object GetPrevious(object relativeTo);
        public object SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                // this will use the Equals method if it is overridden for the data source item class      
                if (!object.Equals(this.selectedItem, value))
                {
                    // save the previously selected item so that we can use it    
                    // to construct the event arguments for the SelectionChanged event    
                    object previousSelectedItem = this.selectedItem;
                    this.selectedItem = value;
                    // fire the SelectionChanged event        
                    this.OnSelectionChanged(previousSelectedItem, this.selectedItem);
                }
            }
        }
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        protected virtual void OnSelectionChanged(object oldSelectedItem, object newSelectedItem)
        {
            EventHandler<SelectionChangedEventArgs> handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs(new object[] { oldSelectedItem }, new object[] { newSelectedItem }));
            }
        }
        #endregion
    }

    public class LoopingArrayDataSource<T> : LoopingDataSourceBase
    {
        Dictionary<T, int> entries;

        /// <summary>
        /// 数组数据源
        /// </summary>
        public T[] DataSource { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array">数组数据源</param>
        /// <param name="index">初始选中的数组元素索引值</param>
        public LoopingArrayDataSource(T[] array, int index = 0)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException();

            DataSource = array;
            Refresh(index);
        }

        /// <summary>
        /// 刷新数组数据
        /// </summary>
        /// <param name="index">刷新后选中的数据元素索引值</param>
        public void Refresh(int index = 0)
        {
            if (index < 0 || index >= DataSource.Length)
                throw new InvalidOperationException();
            UpdateEntries();
            SelectedItem = DataSource[index];
        }

        void UpdateEntries()
        {
            entries = new Dictionary<T, int>(DataSource.Length);
            for (int i = 0; i < DataSource.Length; i++)
            {
                entries.Add(DataSource[i], i);
            }
        }

        #region 改写LoopingSelectorDataSource元素

        public override object GetNext(object relativeTo)
        {
            var idx = entries[(T)relativeTo] + 1;
            if (idx >= DataSource.Length)
                idx = 0;
            return DataSource[idx];
        }

        public override object GetPrevious(object relativeTo)
        {
            var idx = entries[(T)relativeTo] - 1;
            if (idx < 0)
                idx = DataSource.Length - 1;
            return DataSource[idx];
        }


        #endregion

    }
}

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BTB.Core.Base
{
    /// <summary>
    /// Represents an observable collection that supports batch operations.
    /// </summary>
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        #region Public Methods

        /// <summary>
        /// Adds all items in the specified sequence to the collection and raises a single collection change event.
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Items.Add(item);
            }

            RaiseCollectionChanged();
        }

        /// <summary>
        /// Replaces the entire collection with the specified sequence and raises a single collection change event.
        /// </summary>
        public void ClearAndAddRange(IEnumerable<T> items)
        {
            Items.Clear();
            AddRange(items);
        }

        /// <summary>
        /// Removes all matching items from the collection and raises a single collection change event.
        /// </summary>
        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Items.Remove(item);
            }

            RaiseCollectionChanged();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raises property and collection change notifications for a batch operation.
        /// </summary>
        private void RaiseCollectionChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion
    }
}

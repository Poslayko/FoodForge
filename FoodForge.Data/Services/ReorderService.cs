using System.Collections.ObjectModel;

public sealed class ReorderService
{
    public void MoveUp<T>(ObservableCollection<T> items, T item)
        where T : IOrderedItem
    {
        int itemIndex = items.IndexOf(item);
        int previousIndex = itemIndex - 1;

        if (itemIndex <= 0)
        {
            return;
        }

        items.Move(itemIndex, previousIndex);

        RefreshOrders(items);
    }

    public void MoveDown<T>(ObservableCollection<T> items, T item)
        where T : IOrderedItem
    {
        int itemIndex = items.IndexOf(item);
        int nextIndex = itemIndex + 1;

        if (itemIndex >= items.Count - 1)
        {
            return;
        }

        items.Move(itemIndex, nextIndex);

        RefreshOrders(items);
    }

    public void RefreshOrders<T>(ObservableCollection<T> items)
        where T : IOrderedItem
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Order = i + 1;
        }
    }
}
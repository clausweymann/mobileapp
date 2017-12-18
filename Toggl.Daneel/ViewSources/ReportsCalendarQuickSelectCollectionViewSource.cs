﻿using System;
using Foundation;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Binding.iOS.Views;
using Toggl.Daneel.Views.Reports;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class ReportsCalendarQuickSelectCollectionViewSource
        : MvxCollectionViewSource
    {
        private const string cellIdentifier = nameof(ReportsCalendarQuickSelectViewCell);

        public ReportsCalendarQuickSelectCollectionViewSource(
            UICollectionView collectionView) : base(collectionView)
        {
            collectionView.RegisterNibForCell(ReportsCalendarQuickSelectViewCell.Nib, cellIdentifier);
        }

        protected override UICollectionViewCell GetOrCreateCellFor(UICollectionView collectionView, NSIndexPath indexPath, object item)
            => collectionView.DequeueReusableCell(cellIdentifier, indexPath) as UICollectionViewCell;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var cell = GetOrCreateCellFor(collectionView, indexPath, item);
            if (cell is IMvxBindable bindable)
                bindable.DataContext = item;

            return cell;
        }

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
            => ItemsSource.Count();

        protected override object GetItemAt(NSIndexPath indexPath)
            => ItemsSource.ElementAt((int)indexPath.Item);
    }
}

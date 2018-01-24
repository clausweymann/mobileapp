using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;
using Toggl.Foundation.MvvmCross.Collections;

namespace Toggl.Giskard.TemplateSelectors
{
    public sealed class TimeEntriesLogTemplateSelector : IMvxTemplateSelector
    {
        public const int Header = 0;
        public const int Item = 1;
        public const int Footer = 2;

        public int GetItemLayoutId(int fromViewType)
        {
            if (fromViewType == Header)
                return Resource.Layout.TimeEntriesLogFragmentHeader;

            if (fromViewType == Footer)
                return Resource.Layout.TimeEntriesLogFragmentFooter;

            return Resource.Layout.TimeEntriesLogFragmentCell;
        }

        public int GetItemViewType(object forItemObject)
        {
            if (forItemObject == null)
                return Footer;

            if (forItemObject is TimeEntryViewModelCollection)
                return Header;

            return Item;
        }
    }
}

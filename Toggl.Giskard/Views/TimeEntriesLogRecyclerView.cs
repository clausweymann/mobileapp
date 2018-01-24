using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Util;
using Android.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V7.RecyclerView;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Adapters;
using Toggl.Giskard.Extensions;

namespace Toggl.Giskard.Views
{
    [Register("toggl.giskard.views.TimeEntriesLogRecyclerView")]
    public sealed class TimeEntriesLogRecyclerView : MvxRecyclerView
    {
        public TimeEntriesLogRecyclerAdapter TimeEntriesLogAdapter => (TimeEntriesLogRecyclerAdapter)Adapter;

        public IMvxAsyncCommand<TimeEntryViewModel> ContinueCommand
        {
            get => TimeEntriesLogAdapter.ContinueCommand;
            set => TimeEntriesLogAdapter.ContinueCommand = value;
        }

        public TimeEntriesLogRecyclerView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public TimeEntriesLogRecyclerView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public TimeEntriesLogRecyclerView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle, new TimeEntriesLogRecyclerAdapter())
        {
            SetItemViewCacheSize(20);
            DrawingCacheEnabled = true;
            DrawingCacheQuality = DrawingCacheQuality.High;
            NestedScrollingEnabled = false;
            GetLayoutManager().AutoMeasureEnabled = true;

            var callback = new TimeEntriesLogRecyclerViewTouchCallback(context, this);
            ItemTouchHelper mItemTouchHelper = new ItemTouchHelper(callback);
            mItemTouchHelper.AttachToRecyclerView(this);
        }

        private sealed class TimeEntriesLogRecyclerViewTouchCallback : ItemTouchHelper.SimpleCallback
        {
            private readonly Drawable background;
            private readonly Drawable continueDrawable;
            private readonly int continueDrawableMargin;
            private readonly TimeEntriesLogRecyclerView recyclerView;

            public TimeEntriesLogRecyclerViewTouchCallback(Context context, TimeEntriesLogRecyclerView recyclerView)
                : base(0, ItemTouchHelper.Left)
            {
                this.recyclerView = recyclerView;

                var color = new Color(ContextCompat.GetColor(context, Resource.Color.playButtonGreen));
                background = new ColorDrawable(color);
                continueDrawable = ContextCompat.GetDrawable(context, Resource.Drawable.play_white);
                continueDrawableMargin = (int)16.DpToPixels(context);
            }

            public override int GetSwipeDirs(RecyclerView recyclerView, ViewHolder viewHolder)
            {
                if (viewHolder is TimeEntriesLogRecyclerViewHolder timeEntriesLogViewHolder && timeEntriesLogViewHolder.CanSync)
                    return base.GetSwipeDirs(recyclerView, viewHolder);

                return 0;
            }

            public override bool OnMove(RecyclerView recyclerView, ViewHolder viewHolder, ViewHolder target)
                => false;

            public override void OnSwiped(ViewHolder viewHolder, int direction)
            {
                var swipedPosition = viewHolder.AdapterPosition;
                recyclerView.TimeEntriesLogAdapter.ContinueTimeEntry(swipedPosition);
                recyclerView.TimeEntriesLogAdapter.NotifyItemChanged(swipedPosition);
            }

            public override void OnChildDraw(Canvas c, RecyclerView recyclerView,
                                             ViewHolder viewHolder,
                                             float dX, float dY,
                                             int actionState, bool isCurrentlyActive)
            {
                var isInvalidIndex = viewHolder.AdapterPosition == -1;
                var isTimeEntryCell = viewHolder is TimeEntriesLogRecyclerViewHolder;
                if (isInvalidIndex || !isTimeEntryCell) return;

                var itemView = viewHolder.ItemView;

                background.SetBounds(itemView.Right + (int)dX, itemView.Top, itemView.Right, itemView.Bottom);
                background.Draw(c);

                int itemHeight = itemView.Bottom - itemView.Top;
                int intrinsicWidth = continueDrawable.IntrinsicWidth;
                int intrinsicHeight = continueDrawable.IntrinsicWidth;

                int iconLeft = itemView.Right - continueDrawableMargin - intrinsicWidth;
                int iconRight = itemView.Right - continueDrawableMargin;
                int icontTop = itemView.Top + (itemHeight - intrinsicHeight) / 2;
                int iconBottom = icontTop + intrinsicHeight;
                continueDrawable.SetBounds(iconLeft, icontTop, iconRight, iconBottom);

                continueDrawable.Draw(c);

                base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
            }
        }
    }
}
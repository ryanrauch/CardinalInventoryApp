using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace CardinalInventoryApp.Controls
{
    public class HexagonLayout : Layout<View>
    {
        Dictionary<Size, HexagonLayoutData> layoutDataCache = new Dictionary<Size, HexagonLayoutData>();

        public static readonly BindableProperty RadiusProperty =
            BindableProperty.Create("Radius",
                                    typeof(double),
                                    typeof(HexagonLayout),
                                    40.0,
                                    propertyChanged: (bindable, oldvalue, newvalue) =>
                                    {
                                        ((HexagonLayout)bindable).InvalidateLayout();
                                    });

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly BindableProperty PointyTopProperty =
            BindableProperty.Create("PointyTop",
                                    typeof(bool),
                                    typeof(HexagonLayout),
                                    true,
                                    propertyChanged: (bindable, oldvalue, newvalue) =>
                                    {
                                        ((HexagonLayout)bindable).InvalidateLayout();
                                    });

        public bool PointyTop
        {
            get { return (bool)GetValue(PointyTopProperty); }
            set { SetValue(PointyTopProperty, value); }
        }

        public static readonly BindableProperty OriginTopLeftProperty =
            BindableProperty.Create("OriginTopLeft",
                                    typeof(bool),
                                    typeof(HexagonLayout),
                                    true,
                                    propertyChanged: (bindable, oldvalue, newvalue) =>
                                    {
                                        ((HexagonLayout)bindable).InvalidateLayout();
                                    });
        public bool OriginTopLeft
        {
            get { return (bool)GetValue(OriginTopLeftProperty); }
            set { SetValue(OriginTopLeftProperty, value); }
        }

        public static readonly BindableProperty IsMenuProperty =
            BindableProperty.Create("IsMenu",
                                    typeof(bool),
                                    typeof(HexagonLayout),
                                    false,
                                    propertyChanged: (bindable, oldvalue, newvalue) =>
                                    {
                                        ((HexagonLayout)bindable).InvalidateLayout();
                                    });
        public bool IsMenu
        {
            get { return (bool)GetValue(IsMenuProperty); }
            set { SetValue(IsMenuProperty, value); }
        }

        public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create(
              "ColumnSpacing",
              typeof(double),
              typeof(HexagonLayout),
              5.0,
              propertyChanged: (bindable, oldvalue, newvalue) =>
              {
                  ((HexagonLayout)bindable).InvalidateLayout();
              });

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create(
              "RowSpacing",
              typeof(double),
              typeof(HexagonLayout),
              5.0,
              propertyChanged: (bindable, oldvalue, newvalue) =>
              {
                  ((HexagonLayout)bindable).InvalidateLayout();
              });

        public double RowSpacing
        {
            get { return (double)GetValue(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(HexagonLayout),
                null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ItemsChanged
            );

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(HexagonLayout),
                default(DataTemplate),
                propertyChanged: (bindable, oldValue, newValue) => {
                    var control = (HexagonLayout)bindable;
                    //when to occur propertychanged earlier ItemsSource than ItemTemplate, raise ItemsChanged manually
                    if (newValue != null && control.ItemsSource != null && !control.doneItemSourceChanged)
                    {
                        ItemsChanged(bindable, null, control.ItemsSource);
                    }
                }
            );

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject pObj, object pOldVal, object pNewVal)
        {
            var layout = pObj as HexagonLayout;

            if (layout != null && layout.ItemTemplate != null)
                layout.BuildLayout();
        }

        private void BuildLayout()
        {
            Children.Clear();
            if (ItemsSource == null)
            {
                return;
            }
            foreach (var item in ItemsSource)
            {
                var view = (View)ItemTemplate.CreateContent();
                view.BindingContext = item;
                Children.Add(view);
            }
        }

        private double HexagonHeight
        {
            get
            {
                if (PointyTop)
                {
                    return Radius * 2;
                }
                else
                {
                    return Radius * Math.Cos(Math.PI / 180 * 30) * 2;
                }
            }
        }

        private double HexagonWidth
        {
            get
            {
                if (PointyTop)
                {
                    return Radius * Math.Cos(Math.PI / 180 * 30) * 2;
                }
                else
                {
                    return Radius * 2;
                }
            }
        }

        private HexagonLayoutData GetLayoutData(double width, double height)
        {
            Size size = new Size(width, height);

            // Check if cached information is available.
            if (layoutDataCache.ContainsKey(size))
            {
                return layoutDataCache[size];
            }

            int visibleChildCount = 0;
            Size maxChildSize = new Size();
            int rows = 0;
            int columns = 0;
            HexagonLayoutData layoutData = new HexagonLayoutData();

            /*
            //Enumerate through all the children.
            foreach (View child in Children)
            {
                // Skip invisible children.
                if (!child.IsVisible)
                    continue;

                // Count the visible children.
                visibleChildCount++;

                // Get the child's requested size.
                SizeRequest childSizeRequest = child.Measure(Double.PositiveInfinity, Double.PositiveInfinity);

                // Accumulate the maximum child size.
                maxChildSize.Width = Math.Max(maxChildSize.Width, childSizeRequest.Request.Width);
                maxChildSize.Height = Math.Max(maxChildSize.Height, childSizeRequest.Request.Height);
            }
            */

            // For now force children into a size determined by Radius property
            maxChildSize.Width = HexagonWidth;
            maxChildSize.Height = HexagonHeight;
            visibleChildCount = Children.Where(c => c.IsVisible).Count();

            if (visibleChildCount != 0)
            {
                // Calculate the number of rows and columns.
                if (Double.IsPositiveInfinity(width)
                    || IsMenu)
                {
                    columns = visibleChildCount;
                    rows = 1;
                }
                else
                {
                    //columns = (int)((width + ColumnSpacing) / (maxChildSize.Width + ColumnSpacing));
                    columns = (int)((width - (maxChildSize.Width / 2)) / (maxChildSize.Width + ColumnSpacing));
                    columns = Math.Max(1, columns);
                    rows = (visibleChildCount + columns - 1) / columns;
                }

                /*
                // Now maximize the cell size based on the layout size.
                Size cellSize = new Size();

                if (Double.IsPositiveInfinity(width))
                    cellSize.Width = maxChildSize.Width;
                else
                    cellSize.Width = (width - ColumnSpacing * (columns - 1)) / columns;

                if (Double.IsPositiveInfinity(height))
                    cellSize.Height = maxChildSize.Height;
                else
                    cellSize.Height = (height - RowSpacing * (rows - 1)) / rows;
                */

                // Keep the cells pre-defined size based off of radius
                Size cellSize = new Size(HexagonWidth,
                                         HexagonHeight);

                layoutData = new HexagonLayoutData(visibleChildCount, cellSize, rows, columns);
            }

            layoutDataCache.Add(size, layoutData);
            return layoutData;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            HexagonLayoutData layoutData = GetLayoutData(widthConstraint, heightConstraint);
            if (layoutData.VisibleChildCount == 0)
            {
                return new SizeRequest();
            }
            if (PointyTop)
            {
                return new SizeRequest(new Size(layoutData.CellSize.Width * layoutData.Columns + ColumnSpacing * (layoutData.Columns - 1) + (layoutData.CellSize.Width / 2),
                                                layoutData.CellSize.Height * layoutData.Rows + RowSpacing * (layoutData.Rows - 1) + (layoutData.CellSize.Height / 2)));
            }
            Size totalSize = new Size(layoutData.CellSize.Width * layoutData.Columns + ColumnSpacing * (layoutData.Columns - 1) + HexagonWidth / 2,
                                      layoutData.CellSize.Height * layoutData.Rows + RowSpacing * (layoutData.Rows - 1));
            return new SizeRequest(totalSize);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            HexagonLayoutData layoutData = GetLayoutData(width, height);

            if (layoutData.VisibleChildCount == 0)
            {
                return;
            }

            double halfWidth = HexagonWidth / 2;
            double quarterWidth = halfWidth / 2;
            double halfHeight = HexagonHeight / 2;
            double quarterHeight = halfHeight / 2;

            double xChild = x;
            double yChild = y;

            if (PointyTop)
            {
                //Start off with first element placed at 0.5w-1.5w x 0.5h-1.5h
                xChild += halfWidth + ColumnSpacing / 2;
                if (OriginTopLeft)
                {
                    yChild += halfHeight;
                }
                else
                {
                    if (IsMenu)
                    {
                        xChild = x; //shift menu to far bottom-left corner
                        yChild = height - halfHeight;
                    }
                    else
                    {
                        yChild = height - HexagonHeight;
                    }
                }
            }
            else
            {
                //Start off with first element placed at 0.75w-1.75w x 0h-1h
                xChild += halfWidth + quarterWidth;
            }
            int row = 0;
            int column = 0;
            bool even = true;

            foreach (View child in Children)
            {
                if (!child.IsVisible)
                {
                    continue;
                }
                LayoutChildIntoBoundingRegion(child, new Rectangle(new Point(xChild, yChild), layoutData.CellSize));
                if (PointyTop)
                {
                    if (++column == layoutData.Columns)
                    {
                        column = 0;
                        row++;
                        even = row % 2 == 0;
                        if (even)
                        {
                            //Shift even rows to the right by .5
                            xChild = x + halfWidth + ColumnSpacing / 2;
                        }
                        else
                        {
                            xChild = x;
                        }
                        if (OriginTopLeft)
                        {
                            yChild += RowSpacing + layoutData.CellSize.Height - quarterHeight;
                        }
                        else
                        {
                            yChild -= (RowSpacing + layoutData.CellSize.Height - quarterHeight);
                        }
                    }
                    else
                    {
                        xChild += ColumnSpacing + layoutData.CellSize.Width;
                    }
                }
                else
                {
                    //flat-top
                    if (++column == layoutData.Columns)
                    {
                        column = 0;
                        row++;
                        xChild = x + halfWidth + quarterWidth;
                        if (OriginTopLeft)
                        {
                            yChild += RowSpacing + layoutData.CellSize.Height;
                        }
                        else
                        {
                            yChild -= RowSpacing + layoutData.CellSize.Height;
                        }
                    }
                    else
                    {
                        xChild += ColumnSpacing + layoutData.CellSize.Width - quarterWidth;
                        even = column % 2 == 0;
                        if (even)
                        {
                            //if added .5h, remove it for even columns to shift back down
                            if (column > 0)
                            {
                                if (OriginTopLeft)
                                {
                                    yChild -= halfHeight;
                                }
                                else
                                {
                                    yChild += halfHeight;
                                }
                            }
                        }
                        else
                        {
                            //shift odd-columns up by .5h
                            if (OriginTopLeft)
                            {
                                yChild += halfHeight;
                            }
                            else
                            {
                                yChild -= halfHeight;
                            }
                        }
                    }
                }
            }
        }

        protected override void InvalidateLayout()
        {
            //to bypass this override the ShouldInvalidateOnChildAdded() and ShouldInvalidateOnChildRemoved() methods
            base.InvalidateLayout();
            layoutDataCache.Clear();
        }

        protected override void OnChildMeasureInvalidated()
        {
            base.OnChildMeasureInvalidated();
            layoutDataCache.Clear();
        }

        //start of copied section
        private bool doneItemSourceChanged = false;

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (HexagonLayout)bindable;
            // when to occur propertychanged earlier ItemsSource than ItemTemplate, do nothing.
            if (control.ItemTemplate == null)
            {
                control.doneItemSourceChanged = false;
                return;
            }

            control.doneItemSourceChanged = true;

            IEnumerable newValueAsEnumerable;
            try
            {
                newValueAsEnumerable = newValue as IEnumerable;
            }
            catch (Exception e)
            {
                throw e;
            }

            var oldObservableCollection = oldValue as INotifyCollectionChanged;

            if (oldObservableCollection != null)
            {
                oldObservableCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
            }

            var newObservableCollection = newValue as INotifyCollectionChanged;

            if (newObservableCollection != null)
            {
                newObservableCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
            }

            control.Children.Clear();

            if (newValueAsEnumerable != null)
            {
                foreach (var item in newValueAsEnumerable)
                {
                    var view = CreateChildViewFor(control.ItemTemplate, item, control);

                    control.Children.Add(view);
                }
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {

                this.Children.RemoveAt(e.OldStartingIndex);

                var item = e.NewItems[e.NewStartingIndex];
                var view = CreateChildViewFor(this.ItemTemplate, item, this);

                this.Children.Insert(e.NewStartingIndex, view);
            }

            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    for (var i = 0; i < e.NewItems.Count; ++i)
                    {
                        var item = e.NewItems[i];
                        var view = CreateChildViewFor(this.ItemTemplate, item, this);

                        this.Children.Insert(i + e.NewStartingIndex, view);
                    }
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    this.Children.RemoveAt(e.OldStartingIndex);
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Children.Clear();
            }

            else
            {
                return;
            }

        }

        private View CreateChildViewFor(object item)
        {
            this.ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);
            return (View)this.ItemTemplate.CreateContent();
        }

        private static View CreateChildViewFor(DataTemplate template, object item, BindableObject container)
        {
            var selector = template as DataTemplateSelector;

            if (selector != null)
            {
                template = selector.SelectTemplate(item, container);
            }
            //Binding context
            template.SetValue(BindableObject.BindingContextProperty, item);
            return (View)template.CreateContent();
        }
    }
}

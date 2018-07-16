using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CardinalInventoryApp.Controls
{
    public class SwipeImage : Image
    {
        public static readonly BindableProperty SwipedLeftCommandProperty =
            BindableProperty.Create(
                nameof(SwipedLeftCommand),
                typeof(ICommand),
                typeof(SwipeImage),
                null);

        public ICommand SwipedLeftCommand
        {
            get => (ICommand)GetValue(SwipedLeftCommandProperty);
            set => SetValue(SwipedLeftCommandProperty, value);
        }

        public static readonly BindableProperty SwipedRightCommandProperty =
            BindableProperty.Create(
                nameof(SwipedRightCommand),
                typeof(ICommand),
                typeof(SwipeImage),
                null);

        public ICommand SwipedRightCommand
        {
            get => (ICommand)GetValue(SwipedRightCommandProperty);
            set => SetValue(SwipedRightCommandProperty, value);
        }

        private bool _ignoreTouch = false;

        private float _cardDistance = 0;   // Distance the card has been moved

        private const int _animationLength = 250; // Speed of the animations

        private const float _cardRotationAdjuster = 0.3f; // Higher the number less the rotation effect

        private float _cardMoveDistance { get; set; } = 50f;

        public SwipeImage()
        {
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += this.OnPanUpdated;
            this.GestureRecognizers.Add(panGesture);
            this.PropertyChanged += Source_PropertyChanged;
        }

        private void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Immediately move the back to the center
            if(e.PropertyName == SourceProperty.PropertyName)
            {
                this.TranslateTo(-X, -Y, 0, null);
                this.RotateTo(0, 0, null);
            }
            else if(e.PropertyName == WidthProperty.PropertyName)
            {
                _cardMoveDistance = (float)Width / 4.0f;
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    HandleTouchStart();
                    break;
                case GestureStatus.Running:
                    HandleTouch((float)e.TotalX);
                    break;
                case GestureStatus.Completed:
                    HandleTouchEnd();
                    break;
            }
        }

        // Hande when a touch event begins
        private void HandleTouchStart()
        {
            _cardDistance = 0;
        }

        // Handle the ongoing touch event as the card is moved
        private void HandleTouch(float differenceX)
        {
            if (_ignoreTouch)
            {
                return;
            }

            // Move the card
            TranslationX = differenceX;

            // Calculate a angle for the card
            var rotationAngle = (float)(_cardRotationAdjuster * Math.Min(differenceX / Width, 1.0f));
            Rotation = rotationAngle * 180 / Math.PI;

            // Keep a record of how far its moved
            _cardDistance = differenceX;
        }

        // Handle the end of the touch event
        private async void HandleTouchEnd()
        {
            _ignoreTouch = true;

            // If the card was move enough to be considered swiped off
            if (Math.Abs((int)_cardDistance) > _cardMoveDistance)
            {
                // move off the screen
                await this.TranslateTo(_cardDistance > 0 ? Width : -Width,
                                       0,
                                       _animationLength,
                                       Easing.SinIn);
                if (SwipedRightCommand != null && _cardDistance > 0)
                {
                    SwipedRightCommand.Execute(true);
                }
                else if (SwipedLeftCommand != null)
                {
                    SwipedLeftCommand.Execute(true);
                }
            }
            else
            {
                // Move the top card back to the center
                await Task.WhenAll(this.TranslateTo(-X, -Y, _animationLength, Easing.SinIn),
                                   this.RotateTo(0, _animationLength, Easing.SinIn));
            }
            _ignoreTouch = false;
        }
    }
}

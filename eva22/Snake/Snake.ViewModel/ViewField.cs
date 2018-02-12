using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace Snake.ViewModel {
    /// <summary>
    /// Type of a field.
    /// </summary>
    public class ViewField : INotifyPropertyChanged {

        /// <summary>
        /// Stores the color of the field
        /// </summary>
        Color color;

        /// <summary>
        /// Gets raised when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises a new PropertyChanged event with the name of the property as the argument.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged([CallerMemberName] String propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets or sets the color of the field.
        /// </summary>
        public Color Color {
            get { return color; }
            set {
                if (color != value) {
                    color = value;
                    OnPropertyChanged();
                }
            }
        }


    }
}

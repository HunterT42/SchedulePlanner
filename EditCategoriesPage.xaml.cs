using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage; // Для Preferences
using System.Text.Json; // Для сериализации списка

namespace SchedulePlannerApp
{
    public partial class EditCategoriesPage : ContentPage
    {
        private ObservableCollection<string> _categories; // Коллекция категорий

        public EditCategoriesPage()
        {
            InitializeComponent();
            _categories = LoadCategories(); // Загружаем категории из памяти
            CategoryListView.ItemsSource = _categories;
        }

        // Добавление новой категории
        private void OnAddCategoryClicked(object sender, EventArgs e)
        {
            string newCategory = NewCategoryEntry.Text?.Trim(); // Получаем текст из поля ввода

            if (!string.IsNullOrEmpty(newCategory)) // Проверяем, не пустое ли значение
            {
                if (!_categories.Contains(newCategory)) // Проверяем, нет ли уже такой категории
                {
                    _categories.Add(newCategory); // Добавляем в список
                    SaveCategories(); // Сохраняем изменения
                    NewCategoryEntry.Text = string.Empty; // Очищаем поле ввода
                }
                else
                {
                    DisplayAlert("Ошибка", "Такая категория уже существует.", "OK"); // Предупреждение
                }
            }
            else
            {
                DisplayAlert("Ошибка", "Введите название категории.", "OK"); // Ошибка, если поле пустое
            }
        }

        // Удаление выбранной категории
        private void OnDeleteCategoryClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string categoryToDelete)
            {
                // Подтверждение удаления
                DisplayAlert("Подтверждение",
                    $"Удалить категорию '{categoryToDelete}'?", "Да", "Нет").ContinueWith(task =>
                    {
                        if (task.Result) // Если пользователь нажал "Да"
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                _categories.Remove(categoryToDelete);
                                SaveCategories(); // Сохраняем изменения
                            });
                        }
                    });
            }
        }


        // Сохранение категорий в память устройства
        private void SaveCategories()
        {
            string json = JsonSerializer.Serialize(_categories); // Конвертируем список в JSON
            Preferences.Set("TaskCategories", json); // Сохраняем в локальное хранилище
        }

        // Загрузка категорий из памяти устройства
        private ObservableCollection<string> LoadCategories()
        {
            if (Preferences.ContainsKey("TaskCategories")) // Проверяем, есть ли сохранённые категории
            {
                string json = Preferences.Get("TaskCategories", string.Empty); // Загружаем JSON
                return JsonSerializer.Deserialize<ObservableCollection<string>>(json) ?? new ObservableCollection<string>(); // Десериализуем список
            }

            // Если данных нет, загружаем категории по умолчанию
            return new ObservableCollection<string> { "Здоровье", "Семья", "Личное" };
        }
    }
}

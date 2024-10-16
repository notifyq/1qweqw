using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static team_project.Api.DownloadService;
using System.Windows.Data;
using System.Windows.Media;
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.UserPages
{
    public class ProductDownloadedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int productId = (int)values[0];
            var productsInfoJson = Properties.Settings.Default["ProductsInfo"] as string;
            var productsInfo = string.IsNullOrEmpty(productsInfoJson)
                ? new List<ProductInfo>()
                : JsonConvert.DeserializeObject<List<ProductInfo>>(productsInfoJson);

            var productInfo = productsInfo.FirstOrDefault(pi => pi.ProductId == productId);
            if (productInfo != null)
            {
                // Получаем последнюю версию продукта с сервера
                ProductUpdate latestUpdate = Task.Run(() => GetLastVersion(productId)).Result;

                // Сравниваем версии
                if (latestUpdate != null)
                {
                    if (latestUpdate.UpdateDate > productInfo.UpdateInfo.UpdateDate)
                    {
                        // Если версии не совпадают, то нужно обновить приложение
                        if (targetType == typeof(Color))
                        {
                            return Colors.Yellow; // Можете выбрать другой цвет для индикации обновления
                        }
                        else if (targetType == typeof(string))
                        {
                            return "Необходимо обновить";
                        }
                    }
                    else
                    {
                        // Если версии совпадают, то обновление не требуется
                        if (targetType == typeof(Color))
                        {
                            return Colors.Green;
                        }
                        else if (targetType == typeof(string))
                        {
                            return "Установлен";
                        }
                    }
                }
            }
            else
            {
                if (targetType == typeof(Color))
                {
                    return Colors.Red;
                }
                else if (targetType == typeof(string))
                {
                    return "Не установлен";
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        ApiProduct api = new ApiProduct();
        private async Task<ProductUpdate> GetLastVersion(int productId)
        {
            try
            {
                ProductUpdate productUpdate = await api.GetLastVersion(productId);
                if (productUpdate == null)
                {
                    return null;
                }
                return productUpdate;
            }
            catch (Exception)
            {

                return null;
            }

        }
    } 
}


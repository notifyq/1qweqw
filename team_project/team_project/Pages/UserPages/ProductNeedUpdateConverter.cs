using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static team_project.Api.DownloadService;
using System.Windows.Data;
using team_project.Api;

namespace team_project.Pages.UserPages
{
    public class ProductNeedUpdateConverter : IValueConverter
    {
        private ApiProduct api = new ApiProduct();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int productId = (int)value;
            var productsInfoJson = Properties.Settings.Default["ProductsInfo"] as string;
            var productsInfo = string.IsNullOrEmpty(productsInfoJson)
                ? new List<ProductInfo>()
                : JsonConvert.DeserializeObject<List<ProductInfo>>(productsInfoJson);

            var productInfo = productsInfo.FirstOrDefault(pi => pi.ProductId == productId);
            if (productInfo != null)
            {
                var latestVersion = Task.Run(() => api.GetLastVersion(productId)).Result;
                return productInfo.UpdateInfo.UpdateDate < latestVersion.UpdateDate ? "Необходимо обновить" : "";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

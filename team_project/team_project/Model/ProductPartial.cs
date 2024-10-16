using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using team_project.Api;

namespace team_project.Model
{
    public partial class Product
    {
        Api.ApiProduct api = new Api.ApiProduct();
        public List<BitmapImage> Images
        {
            get
            {
                List<BitmapImage> _images = Task.Run(() => api.GetImages(ProductId)).Result;
                return _images;
            }
            set
            {
                // Освободить ресурсы изображений перед установкой в null
                /*if (Images != null)
                {
                    foreach (var image in Images)
                    {
                        image.Freeze();
                    }
                }

                // Установить в null, чтобы сборщик мусора мог очистить изображения
                Images = null;*/
            }

        }
        public string Genres 
        {
            get 
            {
                if (ProductGenres.Count > 0)
                {
                    return string.Join(", ", ProductGenres.Select(pg => pg.Genre.GenreName));
                }
                else 
                {
                    return string.Empty;
                }
            } 
        } 
    }
}

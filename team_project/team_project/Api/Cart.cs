using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using team_project.Model;

namespace team_project.Api
{
    public class Cart
    {
        public void AddToCart(int product_id)
        {
            List<int> cart = GetCartList();

            if (cart.Contains(product_id))
            {
                return;
            }
            else
            {
                cart.Add(product_id);
            }
            SaveCart(cart);
        }
        public List<int> GetCartList()
        {
            string cartListJson = Properties.Settings.Default.CartListJson;
            List<int> cartList = JsonConvert.DeserializeObject<List<int>>(cartListJson);

            return cartList;
        }
        public void RemoveFromCart(int product_id)
        {
            List<int> cart = GetCartList();
            if (cart.Count == 0)
            {
                return;
            }
            else
            {
                cart.Remove(product_id);
                SaveCart(cart);
            }
            
        }
        public void SaveCart(List<int> cart)
        {
            string cartListJson = JsonConvert.SerializeObject(cart);
            Properties.Settings.Default.CartListJson = cartListJson;
            Properties.Settings.Default.Save();
        }
        public void ClearCart()
        {
            Properties.Settings.Default.CartListJson = "[]";
            Properties.Settings.Default.Save();
        }
    }
}

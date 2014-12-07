
namespace Nancy.DemoApplication1.Modules
{


    using HP.PC.Presentation;
    using Nancy;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            string product = "hello1";



            Get["/"] = _ => View["index"];

            Get["/capture"] = _ =>
            {
                var response = (Response)getMoment();
                response.ContentType = "application/json";
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                return response;
            };
            Options["/capture"] = _ =>
            {
                var response = (Response)"";
                response.ContentType = "application/json";
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                return response;
            };

        }


        public class ImageResult
        {
            public IPcPicture metadata = null;
            public String imageData = null;
        }
        public string getMoment() {
            using (var sdk = HPPC.CreateLink())
            {

                var moment =  sdk.CaptureMomentAsync().Result;


                // Extract the top-level picture and child images.
                var picture = sdk.ExtractPictureAsync(moment).Result;

                // Extract the top-level outline and child outlines.
                var outline = sdk.ExtractOutlineAsync(moment).Result;
               // DynamicDictionary d = new DynamicDictionary();

                List<ImageResult> images = new List<ImageResult>();
                foreach (var pic in picture.Children)
                {
                    ImageResult imres= new ImageResult();
                    imres.metadata=pic;
                    MemoryStream ms = new MemoryStream();
                    
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(pic.Image));
                    encoder.Save(ms);
                    byte[] bitmapdata = ms.ToArray();
                    

                   // Convert.ToBase64String();
                    imres.imageData = "data:image/png;base64,"+Convert.ToBase64String(bitmapdata); 
                    images.Add(imres);

                }

                string json = JsonConvert.SerializeObject(images);
                return json;
            }
        }


    }


    
}
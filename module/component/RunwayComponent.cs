using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using System.Text.RegularExpressions;
using System.Linq;

using System.IO;
using System.Net;
using System.Net.Cache;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Runway
{
    public class RunwayComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RunwayComponent()
          : base(
              "Runway",
              "RS",
              "Please enter the http address.",
              "Runway",
              "Receive")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Input http", "it", "example http://localhost:8000", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("Data", "d", "show data output", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Info", "i", "show data output", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Error", "e", "show data output", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Run", "R", "Run ", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Output runway Data", "ord", "output runway live data", GH_ParamAccess.item);
            pManager.AddTextParameter("Output runway Info", "ori", "output runway live data", GH_ParamAccess.item);
            pManager.AddTextParameter("Output runway Error", "ore", "output runway live data", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        /// 
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // define parameter

            string mainAddress = "";
            bool dataAddress = false;
            bool Binfo = false;
            bool Berror = false;
            bool Brun = false;
            DA.GetData(0, ref mainAddress);
            DA.GetData(1, ref dataAddress);
            DA.GetData(2, ref Binfo);
            DA.GetData(3, ref Berror);
            DA.GetData(4, ref Brun);

            string responseData = null;
            string responseInfo = null;
            string responseError = null;

            bool isValid = true;

            // Http request implementation (instead of Net.Client). Allows live data between Runway and Grasshopper without results getting stored in Cache, resulting in a non static value.

            if (Brun == true && mainAddress != "" && isValid == true)
            {
                if (dataAddress == true)
                {
                    ExpireSolution(true);

                    // Cache settings that avoid to store data

                    HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                    HttpWebRequest.DefaultCachePolicy = policy;

                    WebRequest request = WebRequest.Create(mainAddress + "/data");

                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    request.CachePolicy = noCachePolicy;

                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            // Get the stream containing content returned by the server.
                            using (Stream dataStream = response.GetResponseStream())
                            {
                                // Open the stream using a StreamReader for easy access.
                                StreamReader reader = new StreamReader(dataStream);

                                // Read the content.
                                responseData = reader.ReadToEnd();

                                request.Abort();
                                dataStream.Close();

                                DA.SetData(0, responseData);
                            }

                        }

                    }
                    catch (WebException ex)
                    {
                        using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There is an issue with the server");
                            isValid = false;
                        }
                    }

                }

                if (Binfo == true)
                {
                    ExpireSolution(true);

                    HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                    HttpWebRequest.DefaultCachePolicy = policy;

                    WebRequest request = WebRequest.Create(mainAddress + "/info");

                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    request.CachePolicy = noCachePolicy;

                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            // Get the stream containing content returned by the server.
                            using (Stream dataStream = response.GetResponseStream())
                            {
                                // Open the stream using a StreamReader for easy access.
                                StreamReader reader = new StreamReader(dataStream);

                                // Read the content.
                                responseInfo = reader.ReadToEnd();

                                request.Abort();
                                dataStream.Close();

                                DA.SetData(1, responseInfo);
                            }

                        }

                    }
                    catch (WebException ex)
                    {
                        using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There is an issue with the server");
                            isValid = false;
                        }
                    }
                }

                if (Berror == true)
                {
                    ExpireSolution(true);

                    HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                    HttpWebRequest.DefaultCachePolicy = policy;

                    WebRequest request = WebRequest.Create(mainAddress + "/error");

                    HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    request.CachePolicy = noCachePolicy;

                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            // Get the stream containing content returned by the server.
                            using (Stream dataStream = response.GetResponseStream())
                            {
                                // Open the stream using a StreamReader for easy access.
                                StreamReader reader = new StreamReader(dataStream);

                                // Read the content.
                                responseError = reader.ReadToEnd();

                                request.Abort();
                                dataStream.Close();

                                DA.SetData(2, responseError);
                            }

                        }

                    }
                    catch (WebException ex)
                    {
                        using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There is an issue with the server");
                            isValid = false;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.runway;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a9c05115-f56a-44d9-bd7b-03dc5832a01f"); }
        }
    }
}

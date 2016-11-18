using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FloatingSearchViews.Suggestions.Models;

namespace MusicMono.Views
{
    unsafe class SearchObjectSearchSugestion : Java.Lang.Object, ISearchSuggestion
    {
        private TextView _body;
        private ImageView _image;
        private Context _ctnx;

        public string Body;

        public SearchObjectSearchSugestion(Context ctnx)
        {
            _ctnx = ctnx;
            _body = new TextView(ctnx);
            _image = new ImageView(ctnx);
        }

        public SearchObjectSearchSugestion(Parcel source)
        {
        }
        public SearchObjectSearchSugestion(Context ctnx, string Body)
        {
            _ctnx = ctnx;
            _body = new TextView(ctnx)
            {
                Text = Body
            };
            this.Body = Body;
            _image = new ImageView(ctnx);
        }
        public new IntPtr Handle
        {
            get
            {
                return base.Handle;
            }
        }

        public int DescribeContents()
        {
            throw new NotImplementedException();
        }

        public new void Dispose()
        {
            _body.Dispose();
            _image.Dispose();
        }

        public string GetBody()
        {
            return _body.Text;
        }

        public IParcelableCreator GetCreator()
        {

            return new SearchObjectSearchSugestionCreator();
        }

        public void SetBodyText(TextView p0)
        {
            _body = p0;
        }

        public bool SetLeftIcon(ImageView p0)
        {
            try
            {
                _image = p0;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            throw new NotImplementedException();
        }
    }
    public class SearchObjectSearchSugestionCreator : Java.Lang.Object, IParcelableCreator
    {
        public Java.Lang.Object CreateFromParcel(Parcel source)
        {
            return new SearchObjectSearchSugestion(source);
        }
        public Java.Lang.Object[] NewArray(int size)
        {
            return new SearchObjectSearchSugestion[size];
        }
    }
}
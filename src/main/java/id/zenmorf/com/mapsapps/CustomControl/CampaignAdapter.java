package id.zenmorf.com.mapsapps.CustomControl;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 23/3/2016.
 */
public class CampaignAdapter extends BaseAdapter {

    String[] titles;
    String[] lengths;
    String[] companies;
    Context context;
    int [] imageCampaigns;

    private static LayoutInflater inflater=null;
    public CampaignAdapter(Context newContext, String[] titleList, String[] lengthList, String[] companyList, int[] campaignImages) {
        // TODO Auto-generated constructor stub
        titles = titleList;
        lengths = lengthList;
        companies = companyList;
        context = newContext;
        imageCampaigns = campaignImages;
        inflater = ( LayoutInflater )context.
                getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    }
    @Override
    public int getCount() {
        // TODO Auto-generated method stub
        return titles.length;
    }

    @Override
    public Object getItem(int position) {
        // TODO Auto-generated method stub
        return position;
    }

    @Override
    public long getItemId(int position) {
        // TODO Auto-generated method stub
        return position;
    }

    public class Holder
    {
        TextView tvCompany;
        TextView tvLength;
        TextView tvTitle;
        ImageView imgCapaign;
    }
    @Override
    public View getView(final int position, View convertView, ViewGroup parent) {
        // TODO Auto-generated method stub
        Holder holder=new Holder();
        View rowView;
        rowView = inflater.inflate(R.layout.campaign_list, null);
        holder.tvTitle=(TextView) rowView.findViewById(R.id.tv_title);
        holder.tvLength=(TextView) rowView.findViewById(R.id.tv_length);
        holder.tvCompany=(TextView) rowView.findViewById(R.id.tv_company);
        holder.imgCapaign=(ImageView) rowView.findViewById(R.id.image_list);
        holder.tvTitle.setText(titles[position]);
        holder.tvLength.setText(lengths[position]);
        holder.tvCompany.setText(companies[position]);
        holder.imgCapaign.setImageResource(imageCampaigns[position]);
        rowView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
                Toast.makeText(context, "You Clicked "+titles[position], Toast.LENGTH_LONG).show();
            }
        });
        return rowView;
    }
}

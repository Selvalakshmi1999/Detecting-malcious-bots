using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Data;

public partial class SelectClickStreamOption : System.Web.UI.Page
{
    SqlConnection con;
    SqlCommand cmd;
    SqlDataReader rs;
    SqlDataAdapter adp;
    DataTable dt;
   
    
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Label1.Text = "";
            Menu m3 = (Menu)Master.FindControl("Menu3");
            m3.Visible = true;
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            con.Open();
            if (!IsPostBack)
            {
                if (Session["UserName"] != null)
                {
                    TextBox1.Text = Session["UserName"].ToString();
                    if (Request.QueryString.Get("PType") != null)
                    {
                        string ptype = Request.QueryString.Get("PType");
                        if (ptype.Equals("Image"))
                        {
                            if (Session["PData"] != null)
                            {

                                ArrayList a = (ArrayList)Session["PData"];
                                TextBox2.Text = a[2].ToString();
                                TextBox3.Text = a[3].ToString();
                                showimage();

                            }
                        }

                        else if (ptype.Equals("Audio"))
                        {
                            if (Session["PData1"] != null)
                            {

                                ArrayList a = (ArrayList)Session["PData1"];
                                TextBox2.Text = a[2].ToString();
                                TextBox3.Text = a[3].ToString();
                                showimage();
                            }
                        }

                        else if (ptype.Equals("Video"))
                        {
                            if (Session["PData2"] != null)
                            {

                                ArrayList a = (ArrayList)Session["PData2"];
                                TextBox2.Text = a[2].ToString();
                                TextBox3.Text = a[3].ToString();
                                showimage();
                            }
                        }

                        else if (ptype.Equals("Message"))
                        {
                            if (Session["PData3"] != null)
                            {
                                ArrayList a = (ArrayList)Session["PData3"];
                                TextBox2.Text = a[2].ToString();
                                TextBox3.Text = a[3].ToString();
                                showimage();

                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Label1.Text = ex.Message;
        }
    }
    void showimage()
    {
        if (ViewState["ImageID"] != null)
            ViewState.Remove("ImageID");


        adp = new SqlDataAdapter("select * from imgtable ", con);
        dt = new DataTable();
        adp.Fill(dt);
        Random r = new Random();
        int no = r.Next(dt.Rows.Count);
        ViewState.Add("ImageID", dt.Rows[no]["imgid"].ToString());
        ImageButton1.ImageUrl = Server .MapPath ("CImage\\"+dt.Rows[no]["imgpath"].ToString());
        showparameter();

    }

    void showparameter()
    {      

        if (ViewState["ImageID"] != null)
        {
            if (ViewState["XPoint"] != null)
            {
                ViewState.Remove("XPoint");
            }
            if (ViewState["YPoint"] != null)
            {
                ViewState.Remove("YPoint");
            }

            adp = new SqlDataAdapter("select * from imgptable where imgid=@imgid", con);
            adp.SelectCommand.Parameters.AddWithValue("imgid", ViewState["ImageID"].ToString());
            dt = new DataTable();
            adp.Fill(dt);
            Random r = new Random();
            int n = r.Next(dt.Rows.Count);

            int xpoint =int.Parse (dt.Rows [n]["xpoint"].ToString ());
            int ypoint = int.Parse(dt.Rows[n]["ypoint"].ToString());
            ViewState.Add("XPoint", xpoint);
            ViewState.Add("YPoint", ypoint);
            string pname = dt.Rows[n]["pname"].ToString();
            Label1.Text = "Click Stream is :" + pname;
        }
    }


    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            if (ViewState["XPoint"] != null && ViewState["YPoint"] != null)
            {
                int xpos = e.X;
                int ypos = e.Y;

                int xpoint = int.Parse(ViewState["XPoint"].ToString());
                int ypoint = int.Parse(ViewState["YPoint"].ToString());
                int x1 =xpoint +10 ;
                int x2 =xpoint -10 ;
                int y1 =ypoint +10;
                int y2 =ypoint -10 ;
               
                if ((xpos <= x1 && xpos >= x2) && (ypos <= y1 && ypos >= y2))
                {
                    ViewState.Add("UserType", "User");                  
                }
                else
                {
                    ViewState.Add("UserType", "Bot");                  
                }

                ImageButton1.Enabled = false;
            }
            else
            {
                Label1.Text = "Some Problem is Occured.So Not Verify Your Data......";

            }
        }
        catch (Exception ex)
        {
            Label1.Text = ex.Message;
        }
    }

    void storedata(ArrayList a)
    {
        cmd = new SqlCommand("select * from ptable where pid=@pid", con);
        cmd.Parameters.AddWithValue("pid", a[2].ToString());
        rs = cmd.ExecuteReader();
        bool b = rs.Read();
        rs.Close();
        cmd.Dispose();
        if (b)
        {
            Label1.Text = "Record Already Inserted.....";
            return;
        }
        cmd = new SqlCommand("insert into ptable values (@uname,@ptype,@pid,@pname,@pimage,@pdesc,@pdate,@utype)", con);
        cmd.Parameters.AddWithValue("uname", a[0].ToString());
        cmd.Parameters.AddWithValue("ptype", a[1].ToString());
        cmd.Parameters.AddWithValue("pid", a[2].ToString());
        cmd.Parameters.AddWithValue("pname", a[3].ToString());
        cmd.Parameters.AddWithValue("pimage", a[4].ToString());
        cmd.Parameters.AddWithValue("pdesc", a[5].ToString());
        cmd.Parameters.AddWithValue("pdate", a[6].ToString());
        cmd.Parameters.AddWithValue("utype", ViewState["UserType"].ToString());
        cmd.ExecuteNonQuery();
        cmd.Dispose();
        Label1.Text = "Post Details Inserted......";

    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        try 
        {
            if (Request.QueryString.Get("PType") != null &&   ViewState["UserType"] != null && Session["UserName"] != null)
            {
                string ptype = Request.QueryString.Get("PType");
                if (ptype.Equals("Image"))
                {
                    if (Session["PData"] != null)
                    {

                        ArrayList a = (ArrayList)Session["PData"];
                        storedata(a);

                      
                    }
                }

                else if (ptype.Equals("Audio"))
                {
                    if (Session["PData1"] != null )
                    {

                        ArrayList a = (ArrayList)Session["PData1"];
                        storedata(a);

                     }
                }

                  else if (ptype.Equals("Video"))
                 {
                    if (Session["PData2"] != null )
                    {

                        ArrayList a = (ArrayList)Session["PData2"];
                        storedata(a);

                     }
                }

                else if (ptype.Equals("Message"))
                {
                    if (Session["PData3"] != null)
                    {
                        ArrayList a = (ArrayList)Session["PData3"];
                        storedata(a);

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Label1.Text = ex.Message;
        }
       
    }
   
}
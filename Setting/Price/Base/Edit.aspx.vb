Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Base_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/base", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("priceid")) Then
            Response.Redirect("~/setting/price/base", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("priceid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCategory.SelectedValue = "" Then
                MessageError(True, "CATEGORY IS REQUIRED !")
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "" Then
                MessageError(True, "METHOD IS REQUIRED !")
                Exit Sub
            End If
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlProductGroup.SelectedValue = "" Then
                MessageError(True, "PRODUCT GROUP IS REQUIRED !")
                Exit Sub
            End If
            If txtHeight.Text = "" Then
                MessageError(True, "HEIGHT IS REQUIRED !")
                Exit Sub
            End If
            If txtWidth.Text = "" Then
                MessageError(True, "WIDTH IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE PriceBases SET Category=@Category, Method=@Method, ProductGroupId=@ProductGroupId, PriceGroupId=@PriceGroupId, Height=@Height, Width=@Width, Price=@Price, Conditional=@Conditional WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@ProductGroupId", ddlProductGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Height", txtHeight.Text)
                        thisCmd.Parameters.AddWithValue("@Width", txtWidth.Text)
                        thisCmd.Parameters.AddWithValue("@Price", txtPrice.Text)
                        thisCmd.Parameters.AddWithValue("@Conditional", txtConditional.Text)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceBases", lblId.Text, Session("LoginId").ToString(), "Price Base Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/base", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base", False)
    End Sub

    Protected Sub BindData(priceId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceBases WHERE Id='" & priceId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/price/base", False)
                Exit Sub
            End If

            BindPriceGroup()
            BindProductGroup()

            ddlCategory.SelectedValue = myData("Category").ToString()
            ddlMethod.SelectedValue = myData("Method").ToString()
            ddlPriceGroup.SelectedValue = myData("PriceGroupId").ToString()
            ddlProductGroup.SelectedValue = myData("ProductGroupId").ToString()
            txtHeight.Text = myData("Height").ToString()
            txtWidth.Text = myData("Width").ToString()
            txtPrice.Text = myData("Price").ToString()
            txtConditional.Text = myData("Conditional").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups ORDER BY Id ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub BindProductGroup()
        ddlProductGroup.Items.Clear()
        Try
            ddlProductGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceProductGroups ORDER BY Id ASC")
            ddlProductGroup.DataTextField = "Name"
            ddlProductGroup.DataValueField = "Id"
            ddlProductGroup.DataBind()

            If ddlProductGroup.Items.Count > 0 Then
                ddlProductGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlProductGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class

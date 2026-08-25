Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Group_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/group/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("pricegroupid")) Then
            Response.Redirect("~/setting/price/group/", False)
            Exit Sub
        End If

        lblId.Text = (Request.QueryString("pricegroupid").ToString())
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "PRICE GROUP NAME IS REQUIRED !")
                Exit Sub
            End If

            If txtName.Text.Trim.ToUpper() <> lblName.Text Then
                Dim checkData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceGroups WHERE Name='" & txtName.Text.Trim.ToUpper() & "'")
                If checkData IsNot Nothing Then
                    MessageError(True, "PRICE GROUP NAME ALREADY EXISTS !")
                    Exit Sub
                End If
            End If

            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlMaster.SelectedValue = "" Then
                MessageError(True, "MASTER IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceGroups ORDER BY Id DESC")
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE PriceGroups SET Name=@Name, Type=@Type, CompanyId=@CompanyId, Master=@Master, Description=@Description, Status=@Status WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim().ToUpper())
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CompanyId", ddlCompany.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Master", ddlMaster.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceGroups", lblId.Text, Session("LoginId").ToString(), "Price Group Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/group", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/group/", False)
    End Sub

    Protected Sub BindData(priceGroupId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceGroups WHERE Id='" & priceGroupId & "'")
            If myData Is Nothing Then Exit Sub

            BindCompany()

            txtName.Text = myData("Name").ToString()
            lblName.Text = myData("Name").ToString()
            ddlType.SelectedValue = myData("Type").ToString()
            ddlCompany.SelectedValue = myData("CompanyId").ToString()
            txtDescription.Text = myData("Description").ToString()
            ddlMaster.SelectedValue = myData("Master").ToString()
            ddlStatus.SelectedValue = myData("Status").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingClass.GetDataTable("SELECT Id, Alias FROM Companys WHERE Status='Active' OR Status='Inactive' ORDER BY Id ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
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

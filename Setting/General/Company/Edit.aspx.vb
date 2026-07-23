Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Company_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/company", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Response.Redirect("~/setting/general/company", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("cid").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "COMPANY NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtAlias.Text = "" Then
                MessageError(True, "COMPANY ALIAS IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Companys SET Name=@Name, Alias=@Alias, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Companys", lblId.Text, Session("LoginId").ToString(), "Company Updated"}
                settingClass.Logs(dataLog)

                Dim url As String = "~/setting/general/company"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/general/company/detail?cid={0}", lblId.Text)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Dim url As String = "~/setting/general/company"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/general/company/detail?cid={0}", lblId.Text)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(companyId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Companys WHERE Id='" & companyId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/general/company", False)
                Exit Sub
            End If

            txtName.Text = thisData("Name").ToString()
            txtAlias.Text = thisData("Alias").ToString()
            txtDescription.Text = thisData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(thisData("Active"))
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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

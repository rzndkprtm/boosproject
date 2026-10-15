Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Zero_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/zero", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("zeroid")) Then
            Response.Redirect("~/setting/general/zero", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("zeroid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE AKZeros SET Name=@Name, ItemCode=@ItemCode, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@ItemCode", txtItemCode.Text)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"AKZeros", lblId.Text, Session("LoginId").ToString(), "AK Zero Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/general/zero", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/general/zero", False)
        Exit Sub
    End Sub

    Protected Sub BindData(xeroId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM AKZeros WHERE Id='" & lblId.Text & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/general/zero", False)
                Exit Sub
            End If

            txtName.Text = myData("Name").ToString()
            txtItemCode.Text = myData("ItemCode").ToString()
            txtDescription.Text = myData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))
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

Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Group_Add
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

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCompany()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "PRICE GROUP NAME IS REQUIRED !")
                Exit Sub
            End If

            Dim checkData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceGroups WHERE Name='" & txtName.Text.Trim().ToUpper() & "'")
            If checkData IsNot Nothing Then
                MessageError(True, "PRICE GROUP NAME ALREADY EXISTS !")
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceGroups ORDER BY Id DESC")
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO PriceGroups VALUES (@Id, @Name, @CompanyId, @Type, @Description, @Active)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim().ToUpper())
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CompanyId", ddlCompany.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceGroups", thisId, Session("LoginId").ToString(), "Price Group Created"}
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

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Id ASC")
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

Imports System.Data.SqlClient

Partial Class Setting_Validation_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/validation", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesign()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesign.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If txtName.Text = "" Then
                MessageError(True, "NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtErrorMessage.Text = "" Then
                MessageError(True, "ERROR MESSAGE IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Validations ORDER BY Id DESC")
                Dim sortOrder As String = settingClass.GetNewSortOrder(ddlDesign.SelectedValue)

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Validations VALUES (@Id, @DesignId, @Name, @ErrorMessage, @SortOrder)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@ErrorMessage", txtErrorMessage.Text)
                        thisCmd.Parameters.AddWithValue("@SortOrder", sortOrder)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Validations", thisId, Session("LoginId").ToString(), "Validation Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/validation", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/validation/", False)
    End Sub

    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
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

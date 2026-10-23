Imports System.Data.SqlClient

Partial Class Journal_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/journal", False)
            Exit Sub
        End If

        If pageAccess = True Then
            If Now.DayOfWeek = DayOfWeek.Saturday OrElse Now.DayOfWeek = DayOfWeek.Sunday Then
                Response.Redirect("~/journal", False)
                Exit Sub
            End If
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer()
            BindForm(ddlType.SelectedValue)
        End If
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindForm(ddlType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlActivity.SelectedValue = "" Then
                MessageError(True, "ACTIVITY IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "CUSTOMER TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "Existing" AndAlso ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "New" AndAlso txtCustomer.Text = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtDescription.Text = "" Then
                MessageError(True, "PRODUCT / DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Journals ORDER BY Id DESC")
                Dim customerData As String = String.Empty
                If ddlType.SelectedValue = "Existing" Then
                    customerData = ddlCustomer.SelectedValue
                End If
                If ddlType.SelectedValue = "New" Then
                    customerData = txtCustomer.Text
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Journals VALUES (@Id, @CreatedBy, GETDATE(), @Activity, @CustomerType, @CustomerData, @Description, NULL, 'Open')", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                        thisCmd.Parameters.AddWithValue("@Activity", ddlActivity.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CustomerType", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CustomerData", customerData)
                        thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                'Dim dataLog As Object() = {"Journals", thisId, Session("LoginId").ToString(), "Journal Created"}
                'settingClass.Logs(dataLog)

                Response.Redirect("~/journal", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/journal", False)
    End Sub

    Protected Sub BindForm(type As String)
        Try
            ddlCustomer.Visible = False
            txtCustomer.Visible = False
            If type = "Existing" Then ddlCustomer.Visible = True
            If type = "New" Then txtCustomer.Visible = True
        Catch ex As Exception
            ddlCustomer.Visible = False
            txtCustomer.Visible = False
        End Try
    End Sub

    Protected Sub BindCustomer()
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Customers WHERE Status='Active' ORDER BY Id ASC")
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 0 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
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

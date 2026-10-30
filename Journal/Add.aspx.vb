Imports System.Data.SqlClient

Partial Class Journal_Add
    Inherits Page

    Dim journalClass As New JournalClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

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
            SetDateValidation()
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
            Dim selectedDate As DateTime
            Dim today As DateTime = DateTime.Today
            Dim startOfWeek As DateTime = today.AddDays(-((CInt(today.DayOfWeek) + 6) Mod 7))

            If Not DateTime.TryParseExact(txtDate.Text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, selectedDate) Then
                MessageError(True, "PLEASE SELECT A VALID DATE.")
                Exit Sub
            End If

            If selectedDate.Date > today Then
                MessageError(True, "FUTURE DATES ARE NOT ALLOWED.")
                Exit Sub
            End If

            If selectedDate.Date < startOfWeek OrElse selectedDate.DayOfWeek = DayOfWeek.Saturday OrElse selectedDate.DayOfWeek = DayOfWeek.Sunday Then
                MessageError(True, "PLEASE SELECT A WORKING DAY WITHIN THE CURRENT WEEK (MONDAY - FRIDAY).")
                Exit Sub
            End If
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
                MessageError(True, "DESCRIPTION IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim thisId As String = journalClass.CreateId()
                Dim customerData As String = String.Empty
                If ddlType.SelectedValue = "Existing" Then
                    customerData = ddlCustomer.SelectedValue
                End If
                If ddlType.SelectedValue = "New" Then
                    customerData = txtCustomer.Text
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Journals VALUES (@Id, @CreatedBy, @CreatedDate, @Activity, @CustomerType, @CustomerData, @Description, NULL, 'Open')", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                        thisCmd.Parameters.AddWithValue("@CreatedDate", selectedDate)
                        thisCmd.Parameters.AddWithValue("@Activity", ddlActivity.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CustomerType", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CustomerData", customerData)
                        thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Journals", thisId, Session("LoginId").ToString(), "Journal Created"}
                journalClass.Logs(dataLog)

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

    Private Sub SetDateValidation()
        Dim today As DateTime = DateTime.Today
        Dim startOfWeek As DateTime = today.AddDays(-((CInt(today.DayOfWeek) + 6) Mod 7))

        txtDate.Text = today.ToString("yyyy-MM-dd")
        txtDate.Attributes("min") = startOfWeek.ToString("yyyy-MM-dd")
        txtDate.Attributes("max") = today.ToString("yyyy-MM-dd")
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
            ddlCustomer.DataSource = journalClass.GetDataTable("SELECT Id, Name FROM Customers WHERE Status='Active' ORDER BY Id ASC")
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

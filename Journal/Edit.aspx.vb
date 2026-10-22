Imports System.Data
Imports System.Data.SqlClient

Partial Class Journal_Edit
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

        If String.IsNullOrEmpty(Request.QueryString("journalid")) Then
            Response.Redirect("~/journal", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("journalid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlType.SelectedValue = "" Then
                MessageError(True, "CUSTOMER TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "Registered" AndAlso ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "Not Registered" AndAlso txtCustomer.Text = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtDescription.Text = "" Then
                MessageError(True, "PRODUCT / DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim customerData As String = String.Empty
                If ddlType.SelectedValue = "Registered" Then
                    customerData = ddlCustomer.SelectedValue
                End If
                If ddlType.SelectedValue = "Not Registered" Then
                    customerData = txtCustomer.Text
                End If

                Dim designId As String = String.Empty
                If Not lbProduct.SelectedValue = "" Then
                    designId = String.Join(",", lbProduct.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Journals SET DesignId=@DesignId, Description=@Description WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@DesignId", designId)
                        thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                'Dim dataLog As Object() = {"Journals", lblId.Text, Session("LoginId").ToString(), "Journal Updated"}
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

    Protected Sub BindData(journalId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Journals WHERE Id='" & journalId & "' AND (Status='Open' OR Status='Closed')")
            If myData Is Nothing Then
                Response.Redirect("~/journal", False)
                Exit Sub
            End If

            BindCustomer()
            BindProduct()

            ddlType.SelectedValue = myData("CustomerType").ToString()
            If ddlType.SelectedValue = "Registered" Then
                ddlCustomer.SelectedValue = myData("CustomerData").ToString()
            End If
            If ddlType.SelectedValue = "Not Registered" Then
                txtCustomer.Text = myData("CustomerData").ToString()
            End If
            txtDescription.Text = myData("Description").ToString()

            If Not myData("DesignId").ToString() = "" Then
                Dim productArray() As String = myData("DesignId").ToString().Split(",")
                For Each i In productArray
                    If Not String.IsNullOrEmpty(i) Then
                        Dim item = lbProduct.Items.FindByValue(i)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If

            ddlCustomer.Visible = False : txtCustomer.Visible = False

            If ddlType.SelectedValue = "Registered" Then ddlCustomer.Visible = True : ddlCustomer.Visible = True
            If ddlType.SelectedValue = "Not Registered" Then txtCustomer.Visible = True : txtCustomer.Visible = True

            ddlType.Enabled = False : ddlCustomer.Enabled = False : txtCustomer.Enabled = False
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindProduct()
        lbProduct.Items.Clear()
        Try
            lbProduct.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS CompanyArray WHERE Active=1 AND Type IN ('Blinds', 'Shutters', 'Doors', 'Samples') AND CompanyArray.VALUE=2 ORDER BY Id ASC")
            lbProduct.DataTextField = "Name"
            lbProduct.DataValueField = "Id"
            lbProduct.DataBind()

            If lbProduct.Items.Count > 0 Then
                lbProduct.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbProduct.Items.Clear()
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

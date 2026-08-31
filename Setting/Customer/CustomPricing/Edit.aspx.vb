Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_CustomPricing_Edit

    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/custompricing", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("customid")) Then
            Response.Redirect("~/setting/customer/custompricing", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("customid").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQURIED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisTags As String = String.Empty
                Dim selected As String = String.Empty
                For Each item As ListItem In lbTags.Items
                    If item.Selected Then
                        selected += item.Text & ","
                    End If
                Next

                If Not selected = "" Then
                    thisTags = selected.Remove(selected.Length - 1).ToString()
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE CustomerCustomPricings WHERE Description=@Tags WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", ddlCustomer.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Tags", thisTags)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerCustomPricings", ddlCustomer.SelectedValue, Session("LoginId").ToString(), "Customer Custom Pricing Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/customer/custompricing", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer/custompricing", False)
    End Sub

    Protected Sub BindData(customId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerCustomPricings WHERE Id='" & customId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/customer/custompricing", False)
                Exit Sub
            End If

            BindCustomer(thisData("Id").ToString())

            ddlCustomer.SelectedValue = thisData("Id").ToString()

            Dim tagsArray() As String = thisData("Description").ToString().Split(",")
            Dim tagsList As List(Of String) = tagsArray.ToList()

            For Each i In tagsArray
                If Not (i.Equals(String.Empty)) Then
                    lbTags.Items.FindByValue(i).Selected = True
                End If
            Next
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCustomer(customerId As String)
        ddlCustomer.Items.Clear()
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@CustomerId", If(customerId Is Nothing, CType(DBNull.Value, Object), customerId)),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            ddlCustomer.DataSource = settingClass.GetDataTableSP("sp_Customers_List_Dropdown", params)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
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

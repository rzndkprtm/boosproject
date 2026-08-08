Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Group_Delete
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
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "NEW PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE PriceGroups SET Status='Deleted', Name=CASE WHEN Name LIKE '%(DELETED)%' THEN Name ELSE Name + ' (DELETED)' END WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.Add("@Id", SqlDbType.Int).Value = CInt(lblId.Text)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"PriceGroups", lblId.Text, Session("LoginId").ToString(), "Price Group Deleted | '" & txtDescription.Text & "'"}
            settingClass.Logs(dataLog)

            Dim fieldTable As String = String.Empty
            If lblType.Text = "Blinds" Then fieldTable = "PriceGroupId"
            If lblType.Text = "Shutters" Then fieldTable = "ShutterPriceGroupId"
            If lblType.Text = "Doors" Then fieldTable = "DoorPriceGroupId"

            If Not String.IsNullOrEmpty(fieldTable) Then
                Dim customerData As DataTable = settingClass.GetDataTable(String.Format("SELECT Id FROM Customers WHERE {0}={1}", fieldTable, lblId.Text))
                If customerData.Rows.Count > 0 Then
                    For i As Integer = 0 To customerData.Rows.Count - 1
                        Dim customerId As String = customerData.Rows(i)(0).ToString()

                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As New SqlCommand(String.Format("UPDATE Customers SET {0}=@NewPrice WHERE Id=@Id", fieldTable), thisConn)
                                thisCmd.Parameters.Add("@Id", SqlDbType.Int).Value = CInt(customerId)
                                thisCmd.Parameters.Add("@NewPrice", SqlDbType.Int).Value = CInt(ddlPriceGroup.SelectedValue)

                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        dataLog = {"Customers", customerId, Session("LoginId").ToString(), "Price Group Updated"}
                        settingClass.Logs(dataLog)
                    Next
                End If
            End If

            Response.Redirect("~/setting/price/group/", False)
            Exit Sub
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
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceGroups WHERE Id='" & priceGroupId & "' AND (Status='Active' OR Status='Inactive')")
            If myData Is Nothing Then
                Response.Redirect("~/setting/price/group/", False)
                Exit Sub
            End If

            BindPriceGroup(priceGroupId)

            txtName.Text = myData("Name").ToString()
            lblType.Text = myData("Type").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup(deletedId As String)
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Id<>'" & deletedId & "' AND (Status='Active' OR Status='Inactive')")
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

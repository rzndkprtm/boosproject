Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Alias_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product/alias", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("aliasid")) Then
            Response.Redirect("~/setting/specification/product/alias", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("aliasid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlFirstId.SelectedValue = "" Then
                MessageError(True, "FIRST PRODUCT IS REQUIRED !")
                Exit Sub
            End If
            If ddlSecondId.SelectedValue = "" Then
                MessageError(True, "SECOND PRODUCT IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE ProductAlias SET FirstId=@FirstId, SecondId=@SecondId WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"ProductAlias", lblId.Text, Session("LoginId").ToString(), "Alias Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/product/alias", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/product/alias", False)
    End Sub

    Protected Sub BindData(aliasId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM ProductAlias WHERE Id='" & aliasId & "'")
            If thisData Is Nothing Then Exit Sub

            BindProduct()

            ddlFirstId.SelectedValue = thisData("FirstID").ToString()
            ddlSecondId.SelectedValue = thisData("SecondID").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindProduct()
        ddlFirstId.Items.Clear() : ddlSecondId.Items.Clear()
        Try
            ddlFirstId.DataSource = settingClass.GetDataTable("SELECT * FROM Products ORDER BY Name ASC")
            ddlFirstId.DataTextField = "Name"
            ddlFirstId.DataValueField = "Id"
            ddlFirstId.DataBind()

            ddlSecondId.DataSource = settingClass.GetDataTable("SELECT * FROM Products ORDER BY Name ASC")
            ddlSecondId.DataTextField = "Name"
            ddlSecondId.DataValueField = "Id"
            ddlSecondId.DataBind()

            If ddlFirstId.Items.Count > 0 Then
                ddlFirstId.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlSecondId.Items.Count > 0 Then
                ddlSecondId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
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

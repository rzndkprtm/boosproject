Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Alias_Add
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

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindProduct()
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
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM ProductAlias ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO ProductAlias VALUES (@Id, @FirstId, @SecondId)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@FirstId", ddlFirstId.SelectedValue)
                        myCmd.Parameters.AddWithValue("@SecondId", ddlSecondId.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"ProductAlias", thisId, Session("LoginId").ToString(), "Alias Created"}
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

    Protected Sub BindProduct()
        ddlFirstId.Items.Clear() : ddlSecondId.Items.Clear()
        Try
            ddlFirstId.DataSource = SettingClass.GetDataTable("SELECT * FROM Products ORDER BY Name ASC")
            ddlFirstId.DataTextField = "Name"
            ddlFirstId.DataValueField = "Id"
            ddlFirstId.DataBind()

            ddlSecondId.DataSource = SettingClass.GetDataTable("SELECT * FROM Products ORDER BY Name ASC")
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

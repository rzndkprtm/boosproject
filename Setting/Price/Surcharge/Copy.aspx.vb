Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Copy
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindPriceGroup()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "FROM PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlPriceGroupNew.SelectedValue = "" Then
                MessageError(True, "TO PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim surchargeData As DataTable = settingClass.GetDataTable("SELECT Id FROM PriceSurcharges WHERE PriceGroupId='" & ddlPriceGroup.SelectedValue & "' AND Active=1 ORDER BY Id ASC")
                If surchargeData.Rows.Count > 0 Then
                    For i As Integer = 0 To surchargeData.Rows.Count - 1
                        Dim surchargeId As String = surchargeData.Rows(i)(0).ToString()

                        Dim newId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceSurcharges ORDER BY Id DESC")

                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As New SqlCommand("INSERT INTO PriceSurcharges SELECT @NewId, DesignId, @NewPriceGroupId, Name, Type, Formula, BuyCharge, SellCharge, Description, Active FROM PriceSurcharges WHERE Id=@Id", thisConn)
                                thisCmd.Parameters.Add("@Id", SqlDbType.Int).Value = CInt(surchargeId)
                                thisCmd.Parameters.Add("@NewId", SqlDbType.Int).Value = CInt(newId)
                                thisCmd.Parameters.Add("@NewPriceGroupId", SqlDbType.Int).Value = CInt(ddlPriceGroupNew.SelectedValue)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using
                    Next
                End If

                Response.Redirect("~/setting/price/surcharge", False)
                Exit Sub
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/surcharge", False)
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        ddlPriceGroupNew.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT Id, Name FROM PriceGroups WHERE Status='Active' ORDER BY Name ASC"

            ddlPriceGroup.DataSource = settingClass.GetDataTable(thisQuery)
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            ddlPriceGroupNew.DataSource = settingClass.GetDataTable(thisQuery)
            ddlPriceGroupNew.DataTextField = "Name"
            ddlPriceGroupNew.DataValueField = "Id"
            ddlPriceGroupNew.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
            If ddlPriceGroupNew.Items.Count > 0 Then
                ddlPriceGroupNew.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
            ddlPriceGroupNew.Items.Clear()
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

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using Firebase.Extensions;

public class FirebaseController : MonoBehaviour
{

    public GameObject PaginaInicioSesion, PaginaRegistro, PaginaPerfil, PaginaRecuperarContrasenia, PaginaNotificacionError;

    public TMP_InputField InputIGmail, InputIContrasenia, InputRNombre, InputRGmail, InputRContrasenia, InputRConfirmarContrasenia, InputGmailRecuperarContrasenia;

    public TMP_Text Error_Label, MensajeError_Label, MostrarNombre, MostrarGmail;

    public Toggle recuerdame;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();

            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    public void AbrirPaginaInicioSesion()
    {
        PaginaInicioSesion.SetActive(true);
        PaginaRegistro.SetActive(false);
        PaginaPerfil.SetActive(false);
        PaginaRecuperarContrasenia.SetActive(false);
        LimpiarCampos();
    }

    public void AbrirPaginaRegistro()
    {
        PaginaInicioSesion.SetActive(false);
        PaginaRegistro.SetActive(true);
        PaginaPerfil.SetActive(false);
        PaginaRecuperarContrasenia.SetActive(false);
        LimpiarCampos();
    }
    public void AbrirPaginaPerfil()
    {
        PaginaInicioSesion.SetActive(false);
        PaginaRegistro.SetActive(false);
        PaginaPerfil.SetActive(true);
        PaginaRecuperarContrasenia.SetActive(false);
        LimpiarCampos();
    }
    public void AbrirPaginaRecuperarContrasenia()
    {
        PaginaInicioSesion.SetActive(false);
        PaginaRegistro.SetActive(false);
        PaginaPerfil.SetActive(false);
        PaginaRecuperarContrasenia.SetActive(true);
        LimpiarCampos();
    }

    public void Logado()
    {
        if (string.IsNullOrEmpty(InputIGmail.text) || string.IsNullOrEmpty(InputIContrasenia.text))
        {
            MostrarMensajes("Error", "Todos los campos son obligatorios");
            return;
        }

        InicioDeSesion(InputIGmail.text, InputIContrasenia.text);
    }

    public void Registrarse()
    {
        if (string.IsNullOrEmpty(InputRNombre.text) || string.IsNullOrEmpty(InputRGmail.text) || string.IsNullOrEmpty(InputRContrasenia.text) || string.IsNullOrEmpty(InputRConfirmarContrasenia.text))
        {
            MostrarMensajes("Error", "Todos los campos son obligatorios");
            return;
        }

        if (InputRContrasenia.text != InputRConfirmarContrasenia.text)
        {
            MostrarMensajes("Error", "Las contraseñas no coinciden");
            return;
        }

        CrearUsuario(InputRGmail.text, InputRContrasenia.text, InputRNombre.text);
    }


    public void RecuperarContrasenia()
    {
        if(string.IsNullOrEmpty(InputGmailRecuperarContrasenia.text))
        {
            MostrarMensajes("Error", "Introduce un correo válido");
            return;
        }

        EnviarSolicitudRecuperarContrasenia(InputGmailRecuperarContrasenia.text);
    }

    public void MostrarMensajes(string titulo, string mensaje)
    {
        Error_Label.text = "" +titulo;
        MensajeError_Label.text = "" + mensaje;

        PaginaNotificacionError.SetActive(true);
    }

    public void CerrarMensajeError()
    {
        Error_Label.text = "";
        MensajeError_Label.text = "";
        PaginaNotificacionError.SetActive(false);
    }

    public void CerrarSesion()
    {
        auth.SignOut();
        MostrarNombre.text = "";
        MostrarGmail.text = "";
        AbrirPaginaInicioSesion();
    }

    void CrearUsuario(string email, string password, string nombre)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception excepcion in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = excepcion as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        MostrarMensajes("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            ActualizarPerfilUsuario(nombre);
        });
    }

    public void InicioDeSesion(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception excepcion in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = excepcion as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        MostrarMensajes("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            MostrarNombre.text = "" + user.DisplayName;
            MostrarGmail.text = "" + user.Email;
            AbrirPaginaPerfil();
        });
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    void ActualizarPerfilUsuario(string nombre)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = nombre,
                PhotoUrl = new System.Uri("https://dummyimage.com/150x150/000/fff.jpg"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");

                MostrarNombre.text = user.DisplayName;
                MostrarGmail.text = user.Email;
                AbrirPaginaPerfil();

                MostrarMensajes("Alerta", "Cuenta creada correctamente");
            });
        }
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Ya existe la cuenta";
                break;
            case AuthError.MissingPassword:
                message = "Hace falta la contraseña";
                break;
            case AuthError.WeakPassword:
                message = "La contraseña es debil";
                break;
            case AuthError.WrongPassword:
                message = "La contraseña es incorrecta";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Ya existe una cuenta con ese correo electrónico";
                break;
            case AuthError.InvalidEmail:
                message = "Correo electronico invalido";
                break;
            case AuthError.MissingEmail:
                message = "Hace falta el correo electrónico";
                break;
            default:
                message = "Ocurrió un error";
                break;
        }
        return message;
    }

    void EnviarSolicitudRecuperarContrasenia(string correoRecuperacion)
    {
        auth.SendPasswordResetEmailAsync(correoRecuperacion).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("La solicitud de restablecimiento de contraseña fue cancelada.");
                return;
            }

            if (task.IsFaulted)
            {
                foreach (Exception excepcion in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = excepcion as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var codigoError = (AuthError)firebaseEx.ErrorCode;
                        MostrarMensajes("Error", GetErrorMessage(codigoError));
                    }
                }
                return;
            }

            MostrarMensajes("Alerta", "Correo para restablecer la contraseña enviado");
        });
    }

    void LimpiarCampos()
    {
        InputIGmail.text = "";
        InputIContrasenia.text = "";
        InputRNombre.text = "";
        InputRGmail.text = "";
        InputRContrasenia.text = "";
        InputRConfirmarContrasenia.text = "";
        InputGmailRecuperarContrasenia.text = "";
    }

}
namespace Server.Data
{
    public enum RoomUserChangeType
    {
        Add = 0,
        Remove = 1,
    }

    public class UserData
    {
        public Guid id { get; set; }
        public string c_nickname { get; set; }
        public string c_email { get; set; }
        public string c_password { get; set; }
        public DateTimeOffset d_registration_date { get; set; }
        public Guid f_authorization_token { get; set; }
        public Guid f_role { get; set; }

        public bool b_is_mail_confirmed { get; set; }
        // public string c_yandex_token { get; set; }
        // public string c_vk_token { get; set; }
        // public string c_google_token { get; set; }

        public UserData()
        {
            id = Guid.NewGuid();
        }
    }

    public class User
    {
        public Guid? id { get; set; }
        public string c_nickname { get; set; }
        public string c_email { get; set; }
        public DateTimeOffset d_registration_date { get; set; }
        public Role f_role { get; set; }
        public bool b_is_mail_confirmed { get; set; }

        public User()
        {
            id = Guid.NewGuid();
        }
    }

    public class UserForCreate
    {
        public string c_nickname { get; set; }
        public string c_email { get; set; }

        public string c_password { get; set; }
        //public string c_yandex_token { get; set; }
        //public string c_vk_token { get; set; }
        //public string c_google_token { get; set; }
    }

    public class Role
    {
        public Guid id { get; set; }
        public string c_name { get; set; }
        public string c_dev_name { get; set; }
        public string? c_description { get; set; }
    }

    public class TripRole
    {
        public Guid id { get; set; }
        public string c_name { get; set; }
        public string c_dev_name { get; set; }
        public string? c_description { get; set; }
        public bool b_is_can_edit { get; set; } = false;
        public bool b_is_can_delete { get; set; } = false;
        public bool b_is_can_invite { get; set; } = false;
        public bool b_is_can_banish { get; set; } = false;
    }

    public class AuthorizationToken
    {
        public Guid id { get; set; }
        public string c_token { get; set; }

        [GraphQLIgnore] public string c_hash { get; set; }

        public AuthorizationToken()
        {
            id = Guid.NewGuid();
        }
    }

    public class Codes
    {
        public Guid id { get; set; }
        public string c_email { get; set; }
        public int n_code { get; set; }
        public DateTime d_expiration_time { get; set; }
    }

    public class ChatsFilterWords
    {
        public Guid id { get; set; }
        public string c_word { get; set; }
        public string c_correctedword { get; set; }
    }

    public class Message
    {
        public Guid id { get; set; }
        public Guid f_sender { get; set; }
        public string c_content { get; set; }
        public DateTime d_datetime { get; set; }
        public Guid f_chat { get; set; }
    }

    public class TripUserListChange
    {
        public Guid userId { get; set; }
        public Guid roomId { get; set; }
        public RoomUserChangeType ChangeType { get; set; }
    }

    public class Trip
    {
        public Guid id { get; set; }
        public string c_name { get; set; }
        public string? c_description { get; set; }
        public DateOnly d_start_date { get; set; }
        public DateOnly d_end_date { get; set; }
        public Guid f_author { get; set; }
    }

    public class TripForCreate
    {
        public string c_name { get; set; }
        public string? c_description { get; set; }
        public DateOnly d_start_date { get; set; }
        public DateOnly d_end_date { get; set; }
    }

    public class UserTrips
    {
        public Guid id { get; set; }
        public Guid f_user_id { get; set; }
        public Guid f_trip_id { get; set; }
        public Guid f_user_trip_role { get; set; }
    }

    public class TripInvites
    {
        public Guid id { get; set; }
        public Guid f_trip_id { get; set; }
        public string c_code { get; set; }
    }
}
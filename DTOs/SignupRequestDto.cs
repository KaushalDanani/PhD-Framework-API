namespace Backend.DTOs
{
    public class SignupRequestDto
    {
        public Step1Dto step1 { get; set; }
        public Step2Dto step2 { get; set; }
        public Step3Dto step3 { get; set; }
        public Step4Dto step4 { get; set; }
    }

    public class Step1Dto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Step2Dto
    {
        public DateTime Dob { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
    }

    public class Step3Dto
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

    public class Step4Dto
    {
        public string RegistrationNo { get; set; }
        public string Password { get; set; }
    }
}

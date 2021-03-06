/*
 * VILT Connector
 *
 * Edge virtual Instructor Led Training (vILT) API Template. Use this template to help generate your API contracts so that you can connect with CSOD and become a vILT provider. Detailed in this API are endpoints that should be implemented so that the contract can adhere to Edge vILT Provider standards.
 *
 * OpenAPI spec version: 1.0.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace vILT.Domain
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class UpdateInstructorRequest : IEquatable<UpdateInstructorRequest>
    { 
        /// <summary>
        /// Current email address of the instructor in the virtual meeting provider&#39;s system.
        /// </summary>
        /// <value>Current email address of the instructor in the virtual meeting provider&#39;s system.</value>
        [Required]
        [DataMember(Name="OldEmail")]
        public string OldEmail { get; set; }

        /// <summary>
        /// New email address of the instructor in the virtual meeting provider&#39;s system.
        /// </summary>
        /// <value>New email address of the instructor in the virtual meeting provider&#39;s system.</value>
        [Required]
        [DataMember(Name="NewEmail")]
        public string NewEmail { get; set; }

        /// <summary>
        /// Instructor&#39;s first name
        /// </summary>
        /// <value>Instructor&#39;s first name</value>
        [Required]
        [DataMember(Name="FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Instructor&#39;s last name
        /// </summary>
        /// <value>Instructor&#39;s last name</value>
        [Required]
        [DataMember(Name="LastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Indicates if the instructor&#39;s record is active.
        /// </summary>
        /// <value>Indicates if the instructor&#39;s record is active.</value>
        [DataMember(Name="IsActive")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class UpdateInstructorRequest {\n");
            sb.Append("  OldEmail: ").Append(OldEmail).Append("\n");
            sb.Append("  NewEmail: ").Append(NewEmail).Append("\n");
            sb.Append("  FirstName: ").Append(FirstName).Append("\n");
            sb.Append("  LastName: ").Append(LastName).Append("\n");
            sb.Append("  IsActive: ").Append(IsActive).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        //public string ToJson()
        //{
        //    //return JsonConvert.SerializeObject(this, Formatting.Indented);
        //}

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((UpdateInstructorRequest)obj);
        }

        /// <summary>
        /// Returns true if UpdateInstructorRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of UpdateInstructorRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UpdateInstructorRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    OldEmail == other.OldEmail ||
                    OldEmail != null &&
                    OldEmail.Equals(other.OldEmail)
                ) && 
                (
                    NewEmail == other.NewEmail ||
                    NewEmail != null &&
                    NewEmail.Equals(other.NewEmail)
                ) && 
                (
                    FirstName == other.FirstName ||
                    FirstName != null &&
                    FirstName.Equals(other.FirstName)
                ) && 
                (
                    LastName == other.LastName ||
                    LastName != null &&
                    LastName.Equals(other.LastName)
                ) && 
                (
                    IsActive == other.IsActive ||
                    IsActive != null &&
                    IsActive.Equals(other.IsActive)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (OldEmail != null)
                    hashCode = hashCode * 59 + OldEmail.GetHashCode();
                    if (NewEmail != null)
                    hashCode = hashCode * 59 + NewEmail.GetHashCode();
                    if (FirstName != null)
                    hashCode = hashCode * 59 + FirstName.GetHashCode();
                    if (LastName != null)
                    hashCode = hashCode * 59 + LastName.GetHashCode();
                    if (IsActive != null)
                    hashCode = hashCode * 59 + IsActive.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(UpdateInstructorRequest left, UpdateInstructorRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UpdateInstructorRequest left, UpdateInstructorRequest right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}

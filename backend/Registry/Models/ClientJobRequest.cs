using Registry.DTO;

namespace Registry.Models
{
    public class ClientJobRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public User InitiatedBy { get; set; } = default!;
        public required Guid InitiatedById { get; set; }
        public DateTime RequestedOn { get; set; } = DateTime.Now;

        // if set, it means that the user doesn't want the tradesman to start the work before this date
        public required DateTime? StartDate { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required bool ShowToEveryone { get; set; }
        public Job? JobApproved { get; set; }
        public Guid? JobApprovedId { get; set; }
        // sets whether this job request is open by the user. This flag no longer in relevant when the job is approved
        public bool Open { get; set; } = true;

        public required List<string> ImagesUrl { get; set; }


    }
}

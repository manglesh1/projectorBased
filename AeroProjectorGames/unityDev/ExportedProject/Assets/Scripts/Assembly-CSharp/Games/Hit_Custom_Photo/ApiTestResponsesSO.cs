using System;
using System.Collections.Generic;
using API.ResponseModels;
using GameSession;
using GameSession.ApiResponses;
using UnityEngine;
using UnityEngine.Networking;

namespace Games.Hit_Custom_Photo.Editor
{
	[CreateAssetMenu(menuName = "API/Test Responses")]
	public class ApiTestResponsesSO : ScriptableObject
	{
		public bool UseFakeResponse { get; set; }

		public bool SetupComplete { get; set; }

		public bool? RequiresApproval { get; set; }

		public bool? AdminApproval { get; set; }

		public bool? TeamApproval1 { get; set; }

		public bool? TeamApproval2 { get; set; }

		public bool? TeamApproval3 { get; set; }

		public bool? TeamApproval4 { get; set; }

		public bool? TeamApproval5 { get; set; }

		public bool? TeamApproval6 { get; set; }

		public ApiResponse<CreateGameSessionApiResponse> CreateGameSessionSuccessfulResponse()
		{
			return new ApiResponse<CreateGameSessionApiResponse>
			{
				Data = new CreateGameSessionApiResponse
				{
					Data = new GameSessionAndUrlModel
					{
						SessionId = "888777",
						UploadImagesSite = "testphotos.com"
					},
					Success = true
				},
				Result = UnityWebRequest.Result.Success
			};
		}

		public ApiResponse<GetGameSessionApiResponse> GetGameSessionApiResponse()
		{
			return new ApiResponse<GetGameSessionApiResponse>
			{
				Data = new GetGameSessionApiResponse
				{
					Data = new GameSessionModel
					{
						Approved = AdminApproval,
						ApprovalRequired = RequiresApproval,
						CreatedAt = DateTime.UtcNow,
						GameSessionId = "888777",
						SetupComplete = SetupComplete,
						Teams = new List<GameSessionTeamModel>
						{
							new GameSessionTeamModel
							{
								Approved = TeamApproval1,
								SetupComplete = SetupComplete,
								Options = new List<GameSessionTeamOptionsModel>
								{
									new GameSessionTeamOptionsModel
									{
										CreatedAt = DateTime.UtcNow,
										GameSessionId = "8887777",
										Value = "https://axeprodstorage.blob.core.windows.net/games/photo-targets/photo-targets-testtarget.png",
										TeamId = 1,
										DataResponseId = "70mCi0bEOQBMgC+gg60vX0saHJTvKTXqYWsESduJgWg=",
										DataTypeKey = "image"
									}
								},
								TeamId = 1,
								DateCreated = DateTime.UtcNow,
								TeamName = "Player 1"
							},
							new GameSessionTeamModel
							{
								Approved = TeamApproval2,
								SetupComplete = SetupComplete,
								Options = new List<GameSessionTeamOptionsModel>
								{
									new GameSessionTeamOptionsModel
									{
										CreatedAt = DateTime.UtcNow,
										GameSessionId = "8887777",
										Value = "https://axeprodstorage.blob.core.windows.net/games/photo-targets/photo-targets-testtarget.png",
										TeamId = 2,
										DataResponseId = "70mCi0bEOQBMgC+gg60vX0saHJTvKTXqYWsESduJgWg=",
										DataTypeKey = "image"
									}
								},
								TeamId = 2,
								DateCreated = DateTime.UtcNow,
								TeamName = "Player 2"
							}
						},
						UploadImagesSite = "testphotos.com"
					},
					Success = true
				},
				Result = UnityWebRequest.Result.Success
			};
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace XService.Business {
    /// <summary>
    /// Provides business logic
    /// </summary>
    public class BusinessEngine : BackgroundService {
        /// <summary>Provides configuration services<summary>
        private readonly IConfiguration _Configuration;

        /// <summary>Provides logging services around the implmentation</summary>
        /// <remarks>
        /// This type of logging is for direct reporting from the business context such as behviors
        /// and execution state. This is not logging as an Aspect but logging by Inversion of Control.
        /// For generalized logging, consider using an Aspect Oriented Programming approach using
        /// interception.
        /// </remarks>
        private readonly ILogger<BusinessEngine> _ExecutionLogger;

        private readonly List<Rules.IRule> _Rules;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public BusinessEngine(IConfiguration configuration, ILogger<BusinessEngine> executionLogger, IEnumerable<Rules.IRule> rules) {
            _Configuration = configuration;
            _ExecutionLogger = executionLogger;
            _Rules = rules.ToList();
        }

        /// <summary>
        /// Runs the business process as a background task
        /// </summary>
        /// <param name="stoppingToken">The task cancellation token</param>
        /// <returns></returns>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
            // This can be used to shutdown the task once the objective has been completed
            var breakExecution = false;

            while (!stoppingToken.IsCancellationRequested && !breakExecution) {
                try {
                    _ExecutionLogger?.LogInformation($"Input Folder: {_Configuration.GetValue<string>("InputFolder")}");
                    _ExecutionLogger?.LogInformation($"Output Folder: {_Configuration.GetValue<string>("OutputFolder")}");

                    // Do work here
                    _ExecutionLogger?.LogInformation("In business logic");
                    _Rules.ForEach(rule => {
                        var model = new Models.SampleModel {
                            Name = "Somebody I UsedToKnow",
                            Created = DateTime.Now
                        };
                        rule.Execute(model);
                    });
                    // force the execution cycle to break
                    breakExecution = true;
                }
                catch (Exception ex) {
                    _ExecutionLogger?.LogError(ex, "An error has occurred");
                    throw;
                }
            }

            _ExecutionLogger?.LogInformation("Shutting down");
        }
    }
}

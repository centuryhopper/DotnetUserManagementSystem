.PHONY: clean run deploy

clean:
	# Reset server wwwroot
	rm -rf ./Server/wwwroot/*
	rmdir ./Server/wwwroot/ || true
	mkdir -p ./Server/wwwroot/

run:
	cd ./Server && \
	( [ -d "./wwwroot" ] && rm -rf ./wwwroot || echo "already removed wwwroot/ from the Server directory" ) && \
	( [ -d "./Models" ] && rm -rf ./Models || echo "already removed Models/ from the Server directory" ) && \
	/usr/bin/dotnet watch run

deploy:
	clean

	# Publish and copy over published contents to server
	cd ./Client && \
	/usr/bin/dotnet publish -o output && \
	cp -r ./output/wwwroot/* ../Server/wwwroot/ && \
	rm -rf ./output/* && \
	rmdir ./output/ || true

	# Copy shared models over to server
	rm -rf ./Server/Models/
	mkdir -p ./Server/Models
	cp -rf ./Shared/Models/* ./Server/Models/

	# Git commit and push
	git add .
	git commit -m "$(m)" || echo "Nothing to commit"
	git push



